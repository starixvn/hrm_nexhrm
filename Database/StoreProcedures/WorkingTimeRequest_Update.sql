CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_Update]
    @Id bigint,
    @IsPlanned bit,
    @Remark nvarchar(2000)= null,
    @FromDate datetime, 
    @FromShift int = null, 
    @ToDate datetime = null, 
    @ToShift int = null,
    @LeaveTypeID int = null, 
    @TotalDays numeric(5,1) = null, 
    @LateInMins int = null,
    @EarlyOutMins int = null,
	@IsCrossDate BIT = 0,
    @ModifiedDate datetime, 
    @ModifiedBy int
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
    Created: 2012/05/08@Dan
    Task: update a request form
    DB: NexHR
    Original: 

    Revisions:
        - 20250507@Henry.Dinh: update HR code to HR-ADM
    Param's Explanation:
*/

BEGIN
    DECLARE @empId INT, @requestType INT,@Status INT

	DECLARE @AnnualLeaveBalance NUMERIC(5, 1)
		,@TotalPedingDays NUMERIC(5, 1)
		,@annualLeaveType INT
		,@callInLeaveType INT
		,@NEXLeaveType INT
		,@BusinessLeave INT
		,@IsValidLeaveDay BIT
		,@OldLeaveTypeId INT
		,@OldFromDate DATETIME
		,@OldToDate DATETIME
		,@OldTotalDays NUMERIC(5, 1)

	DECLARE @HRGroupId INT;
	DECLARE @HeadId INT;
	
	SELECT @HRGroupId = MAX(CASE WHEN ss.Name = 'HRTeamCode' THEN gs.GroupId END)
	FROM bodb_HRM.dbo.GroupSetting as gs WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting ss WITH (NOLOCK) ON ss.Name = 'HRTeamCode'
	WHERE gs.OldGroupCode = ss.Value

	SELECT @HeadId = e.Id
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting ss WITH (NOLOCK) ON ss.Name = 'MGTHeadCode'
	WHERE e.Code = ss.Value

	SELECT @annualLeaveType = 2
		,@callInLeaveType = 30
		,@NEXLeaveType = 31
		,@BusinessLeave = 32

    SELECT @empId = EmpId
		,@requestType = RequestType
		,@Status = [Status]
		,@OldFromDate = FromDate
		,@OldToDate = ToDate
		,@OldLeaveTypeId = LeaveTypeID
		,@OldTotalDays = TotalDays
    FROM dbo.WorkingTimeRequest WITH (NOLOCK)
    WHERE Id = @Id

	IF (@RequestType IN (1, 2))
	BEGIN
		IF (@FromDate IS NULL)
		BEGIN
			SELECT @ErrCode = 101
				,@ErrMsg = 'Missing check in/out request has the invalid parameter'

			RETURN
		END

		IF (
				EXISTS (
					SELECT 1
					FROM dbo.WorkingTimeRequest WITH (NOLOCK)
					WHERE Id <> @Id
						AND empid = @empid
						AND RequestType = @RequestType
						AND STATUS IN (1, 2)
						AND DATEDIFF(d, fromdate, @FromDate) = 0
					)
				)
		BEGIN
			SELECT @ErrCode = 106
				,@ErrMsg = 'There is another request in the same date.'

			RETURN
		END

		IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
					WHERE cl.EmpId = @EmpId
						AND DATEDIFF(DD, cl.WorkDate, @FromDate) = 0
						AND IsHoliday = 0
						AND IsOffDay = 0
					)
				)
		BEGIN
			IF (
					EXISTS (
						SELECT COUNT(1)
						FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
							INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
							AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
						WHERE wk.Id <> @Id
							AND wk.EmpId = @empid
							AND wk.RequestType IN (1, 2)
							AND wk.[Status] IN (1, 2)
							AND DATEDIFF(M, wk.FromDate, @FromDate) = 0
							AND cl.IsHoliday <> 1
							AND cl.IsOffDay <> 1
						HAVING COUNT(1) >= 2
						)
					)
			BEGIN
				IF (
						NOT EXISTS (
							SELECT 1
							FROM dbo.Employee WITH (NOLOCK)
							WHERE Id = @ModifiedBy
								AND GroupId = @HRGroupId
							)
						) -- HR
				BEGIN
					SELECT @ErrCode = 106
						,@ErrMsg = 'The maximum request in a month has reached. You can contact HR for more information.'

					RETURN
				END
				ELSE
				BEGIN
					IF (
							NOT EXISTS (
								SELECT 1
								FROM dbo.Employee WITH (NOLOCK)
								WHERE Id = @EmpId
									AND GroupId = @HRGroupId
								)
							)
					BEGIN
						SELECT @ErrCode = - 1
					END
				END
			END
		END

		-- END
		
		IF(@EmpId = @HeadId) -- user Thien is auto approve check in/out request.
		BEGIN
			SELECT @ErrCode = - 1
		END
	END
    ELSE IF (@RequestType = 6)
    BEGIN
		IF (
				@FromDate IS NULL
				OR @ToDate IS NULL
				OR @LeaveTypeID IS NULL
				OR ISNULL(@TotalDays, 0) <= 0
				)
		BEGIN
			SELECT @ErrCode = 105
				,@ErrMsg = 'Leave request has the invalid parameter'

			RETURN
		END

		-- check range year
		IF (
				(
					@LeaveTypeID = @annualLeaveType
					OR @LeaveTypeID = @callInLeaveType
					)
				AND YEAR(@FromDate) <> YEAR(@ToDate)
				)
		BEGIN
			SELECT @ErrCode = 112
				,@ErrMsg = 'You cannot submit a cross-year leave request. Please split them into 2 requests.'

			RETURN;
		END

		IF (
				(@TotalDays = 0.5 AND
				(
				EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest WITH (NOLOCK)
						WHERE empid = @empid
							AND Id <> @Id
							AND RequestType = @RequestType
							AND TotalDays = 0.5
							AND STATUS IN (1, 2)
							AND (
									(
										DATEDIFF(DD, FromDate, @FromDate) >= 0
										AND DATEDIFF(MI, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, FromDate, @ToDate) >= 0
										AND DATEDIFF(MI, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, FromDate) >= 0
										AND DATEDIFF(MI, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, ToDate) >= 0
										AND DATEDIFF(MI, ToDate, @ToDate) >= 0
									)
						)
					)
				OR EXISTS (
						SELECT 1
							FROM dbo.WorkingTimeRequest WITH (NOLOCK)
							WHERE empid = @empid
								AND Id <> @Id
								AND RequestType = @RequestType
								AND TotalDays <> 0.5
								AND STATUS IN (1, 2)
								AND (
									(
										DATEDIFF(DD, FromDate, @FromDate) >= 0
										AND DATEDIFF(DD, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, FromDate, @ToDate) >= 0
										AND DATEDIFF(DD, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, FromDate) >= 0
										AND DATEDIFF(DD, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(DD, @FromDate, ToDate) >= 0
										AND DATEDIFF(DD, ToDate, @ToDate) >= 0
									)
						)
					)
				)
			)
			OR (
				@TotalDays <> 0.5
				AND EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest WITH (NOLOCK)
						WHERE empid = @empid
							AND Id <> @Id
							AND RequestType = @RequestType
							AND STATUS IN (1, 2)
							AND (
								(
									DATEDIFF(DD, FromDate, @FromDate) >= 0
									AND DATEDIFF(DD, @FromDate, ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, FromDate, @ToDate) >= 0
									AND DATEDIFF(DD, @ToDate, ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, @FromDate, FromDate) >= 0
									AND DATEDIFF(DD, FromDate, @ToDate) >= 0
									)
								OR (
									DATEDIFF(DD, @FromDate, ToDate) >= 0
									AND DATEDIFF(DD, ToDate, @ToDate) >= 0
								)
						)
					)
				)
			)
		BEGIN
			SELECT @ErrCode = 107
				,@ErrMsg = 'There is another request in the same range'

			RETURN
		END

		-- check the total day is valid
		IF (
				@TotalDays < 1
				AND DATEDIFF(d, @FromDate, @ToDate) <> 0
				)
		BEGIN
			SELECT @ErrCode = 109
				,@ErrMsg = 'The total days is not correct'

			RETURN
		END
		--ELSE
		--BEGIN
		--	IF (
		--			@TotalDays >= 1
		--			AND @TotalDays <> (
		--				SELECT COUNT(1)
		--				FROM dbo.EmployeeCalendar
		--				WHERE EmpID = @EmpId
		--					AND WorkDate BETWEEN @FromDate
		--						AND @ToDate
		--					AND IsHoliday = 0
		--					AND IsOffDay = 0
		--					AND IsLeaveDay = 0
		--				)
		--			)
		--	BEGIN
		--		SELECT @ErrCode = 110
		--			,@ErrMsg = 'The total days is not correct'

		--		RETURN
		--	END
		--END

		--SELECT @TotalPedingDays = SUM(TotalDays)
		--FROM dbo.WorkingTimeRequest WITH (NOLOCK)
		--WHERE id <> @Id 
		--	AND EmpId = @EmpId
		--	AND [Status] = 1
		--	AND RequestType = 6
		--	AND (
		--		LeaveTypeID = @annualLeaveType
		--		OR LeaveTypeID = @callInLeaveType
		--		)

		--SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
		--FROM dbo.LeaveTrans WITH (NOLOCK)
		--WHERE EmpId = @EmpId
		--	AND ISNULL(Deleted, 0) = 0
		--	AND YEAR(FromDate) = YEAR(@FromDate)

		IF(@LeaveTypeID = @annualLeaveType OR @LeaveTypeID = @callInLeaveType)
		BEGIN
			SELECT @IsValidLeaveDay = dbo.fnc_CheckValidAnnualLeaveBalanceByRequest(@EmpId, @ToDate, @TotalDays, @Id)

			IF(@IsValidLeaveDay = 0)
			BEGIN
				SELECT @ErrCode = 111
				,@ErrMsg = 'You have exceeded the number of allowed Annual Leave Days that can be taken. Please, submit the �Unpaid Leave� instead.'

				RETURN
			END
		END

		-- Annual leave 2
		IF (@LeaveTypeID = @annualLeaveType)
		BEGIN
			IF (
					(
						@TotalDays = 0.5
						AND DATEDIFF(D, @FromDate, dbo.fnc_GetClientDateTime()) > 0
						)
					OR (
						@TotalDays = 1
						AND 1 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					OR (
						@TotalDays >= 2
						AND @TotalDays < 5
						AND 2 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					OR (
						@TotalDays >= 5
						AND 5 >= (
							SELECT COUNT(1)
							FROM dbo.EmployeeCalendar WITH (NOLOCK)
							WHERE EmpId = @EmpId
								AND DATEDIFF(D, dbo.fnc_GetClientDateTime(), WorkDate) >= 0
								AND DATEDIFF(D, WorkDate, @FromDate) >= 0
								AND IsOffDay <> 1
								AND IsHoliday <> 1
							)
						)
					)
			BEGIN
				SELECT @ErrCode = 112
					,@ErrMsg = 'Request must be made in advance, in accordance with the policy established.'

				RETURN
			END
		END

		IF (@LeaveTypeID = @callInLeaveType)
		BEGIN
			IF (
					NOT EXISTS (
						SELECT 1
						FROM dbo.Employee WITH (NOLOCK)
						WHERE id = @ModifiedBy
							AND (
								Roles = '4'
								OR (Roles LIKE '%1%')
								OR (Roles LIKE '%2%')
								)
						)
					)
			BEGIN
				SELECT @ErrCode = 114
					,@ErrMsg = 'This leave type must be submitted by Leader on behalf of the employee.'

				RETURN
			END
		END

		IF (@LeaveTypeID = @NEXLeaveType)
		BEGIN
			DECLARE @NEXBalance INT
				,@NEXLeavePending NUMERIC(5, 1)

			SELECT @NEXLeavePending = SUM(TotalDays)
			FROM dbo.WorkingTimeRequest WITH (NOLOCK)
			WHERE Id <> @id
				AND EmpId = @EmpId
				AND [Status] = 1
				AND RequestType = 6
				AND LeaveTypeID = @NEXLeaveType

			SELECT @NEXBalance = (oty.[Minutes] - ISNULL(la.[Minutes], 0))
			FROM dbo.OvertimeYearly AS oty WITH (NOLOCK)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON oty.EmpId = la.EmpId
				AND oty.[Year] = la.[Year]
			WHERE oty.EmpId = @EmpId
				AND oty.[Year] = YEAR(@FromDate)

			IF (ISNULL(@NEXBalance, 0) - (ISNULL(@NEXLeavePending, 0) * 480) < (@TotalDays * 480))
			BEGIN
				SELECT @ErrCode = 115
					,@ErrMsg = 'You have not enough NEX days to request.'

				RETURN
			END
		END
    END

    IF(@Status <> 1 OR @empId <> @ModifiedBy)
    BEGIN
       SELECT @ErrCode = 111
                ,@ErrMsg = 'You can not update this request. Please contact HR for more detail.'

            RETURN
    END

	IF (@RequestType = 6)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_EditLeaveRequest_UpdateCalendar] @id
		,@EmpId
		,@LeaveTypeId
		,@FromDate
		,@ToDate
		,@TotalDays
		,@OldLeaveTypeId
		,@OldFromDate
		,@OldToDate
		,@OldTotalDays
		,@ModifiedBy
	END

    UPDATE WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
    SET     
            IsPlanned = @IsPlanned,
            Remark = @Remark ,
            FromDate = @FromDate, 
            FromShift = @FromShift, 
            ToDate = @ToDate, 
            ToShift = @ToShift, 
            LeaveTypeID = @LeaveTypeID, 
            TotalDays = @TotalDays, 
            LateInMins = @LateInMins,
            EarlyOutMins = @EarlyOutMins,
            ModifiedDate = @ModifiedDate,
            ModifiedBy = @ModifiedBy,
			IsCrossDate = @IsCrossDate 
     WHERE Id=@Id

    EXEC [WorkingTimeRequest_WriteHistory] @Id

	IF (@LeaveTypeID = @BusinessLeave AND @EmpId = @HeadId)
	BEGIN
		SELECT @ErrCode = - 1
	END

	IF (
			NOT EXISTS (
				SELECT 
					CASE WHEN es.ApproverId IS NULL THEN gs.ApproverId ELSE es.ApproverId END
				FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON e.GroupId = gs.GroupId AND gs.IsDeleted = 0 AND e.Id <> gs.ApproverId
				LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON e.Id = es.EmpId AND es.IsDeleted = 0 AND e.Id <> es.ApproverId
				WHERE e.Id = @empId
					AND (gs.ApproverId IS NOT NULL OR es.ApproverId IS NOT NULL))
			)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_ApproveRequest] @RequestID = @id
			,@ApprovedBy = @EmpId
			,@ApprovedDate = @ModifiedDate
			,@ErrCode = @ErrCode
			,@ErrMsg = @ErrMsg

		SET @ErrCode = - 1
	END
	ELSE IF (@ErrCode = - 1)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_ApproveRequest] @RequestID = @id
			,@ApprovedBy = @ModifiedBy
			,@ApprovedDate = @ModifiedDate
			,@ErrCode = @ErrCode
			,@ErrMsg = @ErrMsg
	END
	ELSE
	BEGIN
		SET @ErrCode = 0
	END
END

GO
