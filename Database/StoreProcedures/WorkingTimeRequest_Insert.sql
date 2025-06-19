CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_Insert] @id BIGINT OUT
	,@EmpId INT
	,@RequestType SMALLINT
	,@IsPlanned BIT
	,@Remark NVARCHAR(2000) = NULL
	,@FromDate DATETIME
	,@FromShift INT = NULL
	,@ToDate DATETIME = NULL
	,@ToShift INT = NULL
	,@LeaveTypeID INT = NULL
	,@TotalDays NUMERIC(5, 1) = NULL
	,@LateInMins INT = NULL
	,@EarlyOutMins INT = NULL
	,@IsCrossDate BIT = 0
	,@CreatedDate DATETIME
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
	Created: 2012/04/18@Dan
	Task: insert a request form
	DB: NexHR
	Original: 

	Revisions:    
		- 20250507@Henry.Dinh: update HR code to HR-ADM
	Param's Explanation:
*/
BEGIN
	DECLARE @AnnualLeaveBalance NUMERIC(5, 1)
		,@TotalPedingDays NUMERIC(5, 1)
		,@annualLeaveType INT
		,@callInLeaveType INT
		,@NEXLeaveType INT
		,@BusinessLeave INT
		,@IsValidLeaveDay BIT

	DECLARE @HRGroupId INT;
	DECLARE @HeadId INT;

	SELECT @HeadId = e.Id
    FROM dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN dbo.SystemSetting AS ss WITH (NOLOCK) ON ss.Name = 'MGTHeadCode'
    WHERE e.Code = ss.Value

	SELECT @HRGroupId = MAX(CASE WHEN ss.Name = 'HRTeamCode' THEN gs.GroupId END)
	FROM bodb_HRM.dbo.GroupSetting as gs WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting ss WITH (NOLOCK) ON ss.Name = 'HRTeamCode'
	WHERE gs.OldGroupCode = ss.Value

	SELECT @annualLeaveType = 2
		,@callInLeaveType = 30
		,@NEXLeaveType = 31
		,@BusinessLeave = 32

	IF (@RequestType IN (3, 4))
	BEGIN
		IF (DATEDIFF(d, @FromDate, '2015-07-31') < 0)
		BEGIN
			SELECT @ErrCode = 104
				,@ErrMsg = 'You can not request Late in/Early out later than 31 July, 2015.'

			RETURN;
		END
	END

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
					WHERE empid = @empid
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

		-- IF (NOT EXISTS (SELECT 1 FROM dbo.Employee WHERE Id = @CreatedBy AND (Roles = '4' OR (Roles like '%3%'))))
		-- BEGIN
		IF (
				EXISTS (
					SELECT 1
					FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
					WHERE cl.EmpId = @EmpId
						AND DATEDIFF(DD, cl.WorkDate, @FromDate) = 0
						--AND (cl.ShiftId = 14 OR cl.ShiftId IS NULL)
						--AND DATEPART(DW, @FromDate) IN (1, 7)
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
						WHERE wk.EmpId = @empid
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
							WHERE Id = @CreatedBy
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
							AND RequestType = @RequestType
							AND TotalDays = 0.5
							AND STATUS IN (1, 2)
							AND (
									(
										DATEDIFF(MI, FromDate, @FromDate) >= 0
										AND DATEDIFF(MI, @FromDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(MI, FromDate, @ToDate) >= 0
										AND DATEDIFF(MI, @ToDate, ToDate) >= 0
										)
									OR (
										DATEDIFF(MI, @FromDate, FromDate) >= 0
										AND DATEDIFF(MI, FromDate, @ToDate) >= 0
										)
									OR (
										DATEDIFF(MI, @FromDate, ToDate) >= 0
										AND DATEDIFF(MI, ToDate, @ToDate) >= 0
									)
						)
					)
				OR EXISTS (
						SELECT 1
							FROM dbo.WorkingTimeRequest WITH (NOLOCK)
							WHERE empid = @empid
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
		ELSE
		BEGIN
			IF (
					(@TotalDays >= 1
					AND @TotalDays <> (
						SELECT COUNT(1)
						FROM dbo.EmployeeCalendar
						WHERE EmpID = @EmpId
							AND WorkDate BETWEEN @FromDate
								AND @ToDate
							AND IsHoliday = 0
							AND IsOffDay = 0
							AND IsLeaveDay = 0
						))
					OR (@TotalDays = 0.5 AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeCalendar
						WHERE EmpID = @EmpId
							AND WorkDate = CONVERT(DATE, @FromDate) AND WorkDate = CONVERT(DATE, @ToDate)
							AND IsHoliday = 0
							AND IsOffDay = 0
							AND ISNULL(LeaveDay, 0) <> 1
					))
				)
			BEGIN
				SELECT @ErrCode = 110
					,@ErrMsg = 'The total days is not correct'

				RETURN
			END
		END


		
		--IF(@CreatedBy = @EmpId 
		--	OR NOT EXISTS (
		--	SELECT Id 
		--	FROM dbo.Employee WITH (NOLOCK) 
		--	WHERE Id = @CreatedBy AND GroupId = @HRGroupId)
		--)
		--BEGIN
		--	IF(@LeaveTypeID = @annualLeaveType OR @LeaveTypeID = @callInLeaveType)
		--	BEGIN
		--		SELECT @IsValidLeaveDay = dbo.fnc_CheckValidAnnualLeaveBalanceByRequest(@EmpId, @ToDate, @TotalDays, null)

		--		IF(@IsValidLeaveDay = 0)
		--		BEGIN
		--			SELECT @ErrCode = 111
		--			,@ErrMsg = 'You have exceeded the number of allowed Annual Leave Days that can be taken. Please, submit the �Unpaid Leave� instead.'

		--			RETURN
		--		END
		--	END
		--END

		-- check the total day is not greater than annual leave balance
		SELECT @TotalPedingDays = SUM(TotalDays)
		FROM dbo.WorkingTimeRequest WITH (NOLOCK)
		WHERE EmpId = @EmpId
			AND [Status] = 1
			AND RequestType = 6
			AND (
				LeaveTypeID = 2
				OR LeaveTypeID = 30
				)
			AND YEAR(FromDate) = YEAR(@FromDate)

		SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
		FROM dbo.LeaveTrans WITH (NOLOCK)
		WHERE EmpId = @EmpId
			AND ISNULL(Deleted, 0) = 0
			AND YEAR(FromDate) = YEAR(@FromDate)

		IF (
				EXISTS (
					SELECT 1
					FROM dbo.LeaveType
					WHERE Id = @LeaveTypeID
						AND IsALDeduction = 1
					)
				AND (@TotalDays + ISNULL(@TotalPedingDays, 0)) > ISNULL(@AnnualLeaveBalance, 0)
				)
		BEGIN
			SELECT @ErrCode = 111
				,@ErrMsg = 'You have not enough annual leave days to request'

			RETURN
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
						WHERE id = @CreatedBy
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
			WHERE EmpId = @EmpId
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

	INSERT INTO dbo.WorkingTimeRequest (
		STATUS
		,EmpId
		,RequestType
		,IsPlanned
		,Remark
		,CreatedDate
		,CreatedBy
		,ModifiedDate
		,ModifiedBy
		,FromDate
		,FromShift
		,ToDate
		,ToShift
		,LeaveTypeID
		,TotalDays
		,LateInMins
		,EarlyOutMins
		,IsCrossDate
		)
	SELECT 
		1
		,-- pending status as default
		@EmpId
		,@RequestType
		,@IsPlanned
		,@Remark
		,@CreatedDate
		,@CreatedBy
		,@CreatedDate
		,@CreatedBy
		,@FromDate
		,@FromShift
		,@ToDate
		,@ToShift
		,@LeaveTypeID
		,@TotalDays
		,@LateInMins
		,@EarlyOutMins
		,@IsCrossDate
	WHERE NOT EXISTS (
		SELECT 1
		FROM dbo.WorkingTimeRequest WITH (NOLOCK) 
		WHERE EmpId = @EmpId
			AND Status = 1
			AND RequestType = @RequestType
			AND FromDate = @FromDate
			AND ToDate = @ToDate
	)

	SELECT @id = cast(SCOPE_IDENTITY() AS BIGINT)

	EXEC [WorkingTimeRequest_WriteHistory] @id

	IF (@RequestType = 6)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_CreateLeaveRequest_UpdateCalendar] @id
		,@EmpId
		,@LeaveTypeId
		,@FromDate
		,@ToDate
		,@TotalDays
		,@CreatedBy

		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@FromDate
			,@ToDate
			,1
	END

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
			,@ApprovedDate = @CreatedDate
			,@ErrCode = @ErrCode
			,@ErrMsg = @ErrMsg

		SET @ErrCode = - 1
	END
	ELSE IF (@ErrCode = - 1)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_ApproveRequest] @RequestID = @id
			,@ApprovedBy = @CreatedBy
			,@ApprovedDate = @CreatedDate
			,@ErrCode = @ErrCode
			,@ErrMsg = @ErrMsg
	END
	ELSE
	BEGIN
		SET @ErrCode = 0
	END
END
GO
