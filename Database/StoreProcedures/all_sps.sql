USE [NexHRM]
GO

CREATE OR ALTER FUNCTION [dbo].[fnc_CheckValidAnnualLeaveBalanceByRequest] (
	@EmpId INT
	,@ToDate DATETIME
	,@TotalDayOfRequest NUMERIC(18, 1)
	,@RequestId BIGINT = NULL
	)
RETURNS BIT
AS
BEGIN
	DECLARE @IsValid BIT
		,@LimitLeaveBalanceAfterRequest NUMERIC(18, 1)
		,@LeaveDayBalance NUMERIC(18, 1)
		,@CreditDays NUMERIC(18, 1)
		,@LeaveDayOfPendingRequest NUMERIC(18, 1)
		,@TransferDaysFromLastYear NUMERIC(18, 1)
		,@UsedTransferDaysFromLastYear NUMERIC(18, 1)
		,@UsedDaysInFirstQuarter NUMERIC(18, 1)
		,@AnnualLeaveType INT = 2
		,@UnplannedLeaveType INT = 30

	SELECT @LeaveDayOfPendingRequest = SUM(TotalDays)
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE EmpId = @EmpId
		AND [Status] = 1
		AND RequestType = 6
		AND (
			LeaveTypeID = @AnnualLeaveType
			OR LeaveTypeID = @UnplannedLeaveType
			)
		AND YEAR(FromDate) = YEAR(@ToDate)
		AND (@RequestId IS NULL OR id <> @RequestId)

	SELECT @TransferDaysFromLastYear = SUM(ISNULL(CreditDays, 0))
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE EmpId = @EmpId
		AND ISNULL(Deleted, 0) = 0
		AND YEAR(FromDate) = YEAR(@ToDate)
		AND Description = 'Transfer AL from 2024'

	IF(MONTH(@ToDate) < 4)
	BEGIN
		SELECT @LeaveDayBalance = SUM(ISNULL(CreditDays, 0)) - SUM(ISNULL(DeductDays, 0)) - ISNULL(@LeaveDayOfPendingRequest, 0) + ISNULL(@TransferDaysFromLastYear, 0) 
			,@CreditDays = SUM(ISNULL(CreditDays, 0)) + ISNULL(@TransferDaysFromLastYear, 0) 
		FROM dbo.LeaveTrans WITH (NOLOCK)
		WHERE EmpId = @EmpId
			AND ISNULL(Deleted, 0) = 0
			AND YEAR(FromDate) = YEAR(@ToDate)
			AND Description <> 'Transfer AL from 2024'
	END
	ELSE 
	BEGIN
		IF(MONTH(dbo.fnc_GetClientDateTime()) < 4 AND MONTH(@ToDate) = 12)
		BEGIN
			SELECT @LeaveDayBalance = SUM(ISNULL(CreditDays, 0)) - SUM(ISNULL(DeductDays, 0)) - ISNULL(@LeaveDayOfPendingRequest, 0)+ ISNULL(@TransferDaysFromLastYear, 0)
				,@CreditDays = SUM(ISNULL(CreditDays, 0)) + ISNULL(@TransferDaysFromLastYear, 0) 
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND ISNULL(Deleted, 0) = 0
				AND YEAR(FromDate) = YEAR(@ToDate)
				AND Description <> 'Transfer AL from 2024'
		END
		ELSE
		BEGIN
			SELECT @UsedDaysInFirstQuarter = SUM(ISNULL(DeductDays, 0)) + ISNULL(@LeaveDayOfPendingRequest, 0)
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND ISNULL(Deleted, 0) = 0
				AND YEAR(FromDate) = YEAR(@ToDate)
				AND MONTH(FromDate) < 4

			IF(ISNULL(@UsedDaysInFirstQuarter, 0) > ISNULL(@TransferDaysFromLastYear, 0))
			BEGIN
				SET @UsedDaysInFirstQuarter = ISNULL(@TransferDaysFromLastYear, 0)
			END

			SELECT @LeaveDayBalance = SUM(ISNULL(CreditDays, 0)) - SUM(ISNULL(DeductDays, 0)) - ISNULL(@LeaveDayOfPendingRequest, 0) + ISNULL(@UsedDaysInFirstQuarter, 0)
				,@CreditDays = SUM(ISNULL(CreditDays, 0))
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND ISNULL(Deleted, 0) = 0
				AND YEAR(FromDate) = YEAR(@ToDate)
				AND Description <> 'Transfer AL from 2024'
		END
	END

	SELECT @LimitLeaveBalanceAfterRequest = @CreditDays - (FLOOR(@CreditDays * MONTH(@ToDate) / 12) + 5)
	IF (@LimitLeaveBalanceAfterRequest < 0)
	BEGIN
		SET @LimitLeaveBalanceAfterRequest = 0
	END

	-- SELECT @MinLeaveDayMustHaveAfterRequest = CASE WHEN MONTH(@ToDate) < 7 THEN (7 - MONTH(@ToDate)) ELSE 0 END
	IF (MONTH(@ToDate) < 4)
	BEGIN
		SET @IsValid = CASE WHEN @LeaveDayBalance - @TotalDayOfRequest - @LimitLeaveBalanceAfterRequest + ISNULL(@TransferDaysFromLastYear, 0) >= 0 THEN 1 ELSE 0 END
	END
	ELSE
	BEGIN
		SET @IsValid = CASE WHEN @LeaveDayBalance - @TotalDayOfRequest - @LimitLeaveBalanceAfterRequest >= 0 THEN 1 ELSE 0 END
	END
	RETURN @IsValid
END
GO

CREATE OR ALTER FUNCTION [dbo].[fnc_GetAnnualLeaveBalanceByDate] (
	@EmpId INT
	,@ToDate DATETIME
	,@RequestId BIGINT = NULL
	)
RETURNS NUMERIC(18, 1)
AS
BEGIN
	DECLARE @LimitLeaveBalanceAfterRequest NUMERIC(18, 1)
		,@LeaveDayBalance NUMERIC(18, 1)
		,@CreditDays NUMERIC(18, 1)
		,@LeaveDayOfPendingRequest NUMERIC(18, 1)
		,@TransferDaysFromLastYear NUMERIC(18, 1)
		,@UsedTransferDaysFromLastYear NUMERIC(18, 1)
		,@UsedDaysInFirstQuarter NUMERIC(18, 1)
		,@AnnualLeaveType INT = 2
		,@UnplannedLeaveType INT = 30

	SELECT @LeaveDayOfPendingRequest = SUM(TotalDays)
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE EmpId = @EmpId
		AND [Status] = 1
		AND RequestType = 6
		AND (
			LeaveTypeID = @AnnualLeaveType
			OR LeaveTypeID = @UnplannedLeaveType
			)
		AND YEAR(FromDate) = YEAR(@ToDate)
		AND (@RequestId IS NULL OR id <> @RequestId)

	SELECT @TransferDaysFromLastYear = SUM(ISNULL(CreditDays, 0))
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE EmpId = @EmpId
		AND ISNULL(Deleted, 0) = 0
		AND YEAR(FromDate) = YEAR(@ToDate)
		AND Description = 'Transfer AL from 2024'

	IF(MONTH(@ToDate) < 4)
	BEGIN
		SELECT @LeaveDayBalance = SUM(ISNULL(CreditDays, 0)) - SUM(ISNULL(DeductDays, 0)) - ISNULL(@LeaveDayOfPendingRequest, 0) + ISNULL(@TransferDaysFromLastYear, 0) 
			,@CreditDays = SUM(ISNULL(CreditDays, 0)) + ISNULL(@TransferDaysFromLastYear, 0) 
		FROM dbo.LeaveTrans WITH (NOLOCK)
		WHERE EmpId = @EmpId
			AND ISNULL(Deleted, 0) = 0
			AND YEAR(FromDate) = YEAR(@ToDate)
			AND Description <> 'Transfer AL from 2024'
	END
	ELSE 
	BEGIN
		IF(MONTH(dbo.fnc_GetClientDateTime()) < 4 AND MONTH(@ToDate) = 12)
		BEGIN
			SELECT @LeaveDayBalance = SUM(ISNULL(CreditDays, 0)) - SUM(ISNULL(DeductDays, 0)) - ISNULL(@LeaveDayOfPendingRequest, 0)+ ISNULL(@TransferDaysFromLastYear, 0)
				,@CreditDays = SUM(ISNULL(CreditDays, 0)) + ISNULL(@TransferDaysFromLastYear, 0) 
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND ISNULL(Deleted, 0) = 0
				AND YEAR(FromDate) = YEAR(@ToDate)
				AND Description <> 'Transfer AL from 2024'
		END
		ELSE
		BEGIN
			SELECT @UsedDaysInFirstQuarter = SUM(ISNULL(DeductDays, 0)) + ISNULL(@LeaveDayOfPendingRequest, 0)
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND ISNULL(Deleted, 0) = 0
				AND YEAR(FromDate) = YEAR(@ToDate)
				AND MONTH(FromDate) < 4

			IF(ISNULL(@UsedDaysInFirstQuarter, 0) > ISNULL(@TransferDaysFromLastYear, 0))
			BEGIN
				SET @UsedDaysInFirstQuarter = ISNULL(@TransferDaysFromLastYear, 0)
			END

			SELECT @LeaveDayBalance = SUM(ISNULL(CreditDays, 0)) - SUM(ISNULL(DeductDays, 0)) - ISNULL(@LeaveDayOfPendingRequest, 0) + ISNULL(@UsedDaysInFirstQuarter, 0)
				,@CreditDays = SUM(ISNULL(CreditDays, 0))
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND ISNULL(Deleted, 0) = 0
				AND YEAR(FromDate) = YEAR(@ToDate)
				AND Description <> 'Transfer AL from 2024'
		END
	END

	SELECT @LimitLeaveBalanceAfterRequest = @CreditDays - (FLOOR(@CreditDays * MONTH(@ToDate) / 12) + 5)
	IF (@LimitLeaveBalanceAfterRequest < 0)
	BEGIN
		SET @LimitLeaveBalanceAfterRequest = 0
	END

	IF (MONTH(@ToDate) < 4)
	BEGIN
		SET @LeaveDayBalance = @LeaveDayBalance - @LimitLeaveBalanceAfterRequest + ISNULL(@TransferDaysFromLastYear, 0)
	END
	ELSE
	BEGIN
		SET @LeaveDayBalance = @LeaveDayBalance - @LimitLeaveBalanceAfterRequest
	END

	IF (@LeaveDayBalance < 0)
	BEGIN
		SET @LeaveDayBalance = 0
	END

	RETURN ISNULL(@LeaveDayBalance, 0)
END
GO

CREATE OR ALTER FUNCTION [dbo].[fnc_GetAnnualLeaveBalanceByDate_xpre] (
	@EmpId INT
	,@ToDate DATETIME
	,@RequestId BIGINT = NULL
	)
RETURNS NUMERIC(18, 1)
AS
BEGIN
	DECLARE @LimitLeaveBalanceAfterRequest NUMERIC(18, 1)
		,@LeaveDayBalance NUMERIC(18, 1)
		,@CreditDays NUMERIC(18, 1)
		,@LeaveDayOfPendingRequest NUMERIC(18, 1)
		,@AnnualLeaveType INT = 2
		,@UnplannedLeaveType INT = 30

	SELECT @LeaveDayOfPendingRequest = SUM(TotalDays)
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE EmpId = @EmpId
		AND [Status] = 1
		AND RequestType = 6
		AND (
			LeaveTypeID = @AnnualLeaveType
			OR LeaveTypeID = @UnplannedLeaveType
			)
		AND YEAR(FromDate) = YEAR(@ToDate)
		AND (@RequestId IS NULL OR id <> @RequestId)

	SELECT @LeaveDayBalance = SUM(ISNULL(CreditDays, 0)) - SUM(ISNULL(DeductDays, 0)) - ISNULL(@LeaveDayOfPendingRequest, 0)
		,@CreditDays = SUM(ISNULL(CreditDays, 0))
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE EmpId = @EmpId
		AND ISNULL(Deleted, 0) = 0
		AND YEAR(FromDate) = YEAR(@ToDate)

	SELECT @LimitLeaveBalanceAfterRequest = @CreditDays - (FLOOR(@CreditDays * MONTH(@ToDate) / 12) + 5)
	IF (@LimitLeaveBalanceAfterRequest < 0)
	BEGIN
		SET @LimitLeaveBalanceAfterRequest = 0
	END

	SET @LeaveDayBalance = @LeaveDayBalance - @LimitLeaveBalanceAfterRequest

	IF(@LeaveDayBalance < 0)
	BEGIN
		SET @LeaveDayBalance = 0
	END

	RETURN ISNULL(@LeaveDayBalance, 0)
END
GO

CREATE OR ALTER FUNCTION [dbo].[fnc_GetClientDateTime]()
RETURNS DATETIME
AS
BEGIN
    DECLARE @ClientTimeZone NVARCHAR(50) = 'SE Asia Standard Time'; /* SE Asia Standard Time (UTC+7) */
    DECLARE @CurrentTimeZone NVARCHAR(50) = 'SA Western Standard Time'; /* SA Western Standard Time (UTC-4) */
    
    RETURN GETDATE() AT TIME ZONE @CurrentTimeZone AT TIME ZONE @ClientTimeZone
END
GO

CREATE OR ALTER function [dbo].[fnc_getLeaveTypeDefault] () 
returns int
as
begin
	declare @value int
	select top 1 @value = Id
	from dbo.LeaveType 
	where IsDefault = 1

	--if @value is null select @value = dbo.fnc_GetLeaveDayCode()
	return @value
end
GO

CREATE OR ALTER function [dbo].[fnc_GetTableInOut]
(
	@FromTime datetime,
	@ToTime datetime,
	@EmpID int = NULL
) 
returns @tbl_InOut table
		(
			EmpID int
			, TimeIn datetime null
			, TimeOut datetime null
		)
as
/*
Created: 2012/04/09@Dan
Task: get check in/out time for employess
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
begin		
	declare @RecordID numeric(18,0), 
			@EmpIDCur int, 
			@RecordTime datetime, 
			@InOutType bit,
			@IsProcessed bit,
			@CrossShiftTime varchar(5)
	
	set @CrossShiftTime=  '17:00' -- the time that shist startes to be in corss-date

	declare @RecordIDPrev numeric(18,0), 
			@EmpIDPrev char(15), 
			@RecordTimePrev datetime
	declare @InFlag bit, @i int
	
	declare @InOfMachine datetime, @OutOfMachine datetime	

	-- Khai bao Table dung de luu tru du lieu khi duyet, khong lien quan gi den Table chinh TimeRecord
	declare @tbl_TimeRecord table
	( 
		id INT PRIMARY KEY IDENTITY(1,1), 
		RecordID decimal(18, 0)
		, EmpID int
		, RecordTime datetime null
		, InOutType bit null
		, IsProcessed bit null
	)	

	insert @tbl_TimeRecord (RecordID, EmpID, RecordTime, InOutType)
	select 	ID, empID, RecordTime, InOutType
	from 	dbo.TimeRecord with (nolock)
	where	(InOutType is not null)
		and RecordTime> = @FromTime
		-- Quinn edit: Shift M1: check out > 0h yesterday	
		-- and RecordTime< = dateadd(d,1,@ToTime)
		and RecordTime< = convert(varchar(10),dateadd(d,1,@ToTime) ,111)  + ' 07:00' 
		-- end Quinn edit		
		and (@EmpID is null or EmpID=@EmpID)	
	UNION ALL	-- request checkin
	select 	-ID, empID, RequestedCheckinTime as RecordTime, 0  -- use negative to know it's requested time, not from Bioadmin
	from 	dbo.EmployeeCalendar with (nolock)
	where	(RequestedCheckinTime is not null)
		and WorkDate> = @FromTime
		and WorkDate< = @ToTime
		and (@EmpID is null or EmpID=@EmpID)
	UNION ALL	--  request check out time	
	select 	-ID - 10000000, empID, RequestedCheckoutTime, 1 -- use negative to know it's requested time, not from Bioadmin
	from 	dbo.EmployeeCalendar with (nolock)
	where	(RequestedCheckoutTime is not null)
		and WorkDate> = @FromTime
		and WorkDate< = @ToTime
		and (@EmpID is null or EmpID=@EmpID)
	order by EmpID, RecordTime, InOutType	
			
	select @RecordIDPrev = 0, @EmpIDPrev = 0, @RecordTimePrev = null, @i = 1

	WHILE EXISTS (SELECT 1 FROM @tbl_TimeRecord WHERE id = @i)
	BEGIN
		--reset params
		select @InOfMachine= null, @OutOfMachine = null
		
		--get data to process
		SELECT @RecordID = RecordID, @EmpIDCur= EmpID, @RecordTime = RecordTime, @InOutType = InOutType, @IsProcessed = IsProcessed 
		FROM @tbl_TimeRecord WHERE id = @i

		IF (@IsProcessed=1)
		BEGIN 
			SET @i = @i + 1
			continue;
		END 
		--------------------		

		if @EmpIDCur <> @EmpIDPrev
		begin
			-- reset  for current processing emp
			select @InFlag = 0
			select @EmpIDPrev = @EmpIDCur

		end
		if @InOutType = 0  --//In Record
		begin
			
			if @InFlag=0 -- lan check in dau tien
			begin
				select @RecordIDPrev = @RecordID, @RecordTimePrev = @RecordTime
				select @InFlag = 1

				-- neu thoi gian checkin nam trong khoang thoi gian lam viec tren ca 2 ngay (cross date) 
				if (@RecordTimePrev >= cast(CONVERT(VARCHAR(10),@RecordTimePrev,111) + ' ' + @CrossShiftTime as datetime))
				begin									
					--tim thoi gian check out trong ngay ke tiep
					select top 1 @OutOfMachine = RecordTime, @RecordID= ID
					from dbo.TimeRecord WITH (NOLOCK)
					where EmpID = @EmpIDPrev and 
						RecordTime > @RecordTimePrev
						-- Quinn change:
						-- and RecordTime< dateadd(d,1,@RecordTimePrev)
						and RecordTime< dateadd(hh,-8,dateadd(d,1,@RecordTimePrev))
						-- end Quinn change
						and InOutType = 1
					order by RecordTime
					
					-- da du thong tin checkin-checkout
					if (@OutOfMachine is not null)					
					begin						
						insert @tbl_InOut (EmpID, TimeIn, TimeOut)
						values (@EmpIDPrev, @RecordTimePrev, @OutOfMachine)
						
						-- xoa record da xu ly
						delete @tbl_TimeRecord where RecordID = @RecordIDPrev 
						-- marke the records has been process to ignore process later
						update @tbl_TimeRecord 
						set isprocessed=1
						where RecordID = @RecordID

						select @InFlag = 0
					end
				end
			end
			else
			begin
				
				-- checkin nhieu lan trong vong 12h (ma chua checkout), xoa di, chi giu lai 1 lan checkin dau tien
				if (datediff(hh,@RecordTimePrev, @RecordTime) >=0
					and datediff(hh,@RecordTimePrev, @RecordTime)<=12 )
					delete @tbl_TimeRecord where id = @i
				else -- neu checkin nhieu lan nhung ko phai lien tiep (co the la hom sau) thi xu ly binh thuong
					select @RecordIDPrev = @RecordID, @RecordTimePrev = @RecordTime
			end
		end
		else --//Out Record
		begin
			if @InFlag = 1 --//Match one pair
			begin
				select @InOfMachine = @RecordTimePrev, @OutOfMachine = @RecordTime
				-- Can kiem tra so phut keo dai cua ca lam viec
				if (@InOfMachine < @OutOfMachine 
						and datediff(hh,@InOfMachine,@OutOfMachine)<20)
				begin
					insert @tbl_InOut (EmpID, TimeIn, TimeOut)
					values (@EmpIDCur, @InOfMachine, @OutOfMachine)
	
					delete @tbl_TimeRecord where RecordID = @RecordIDPrev OR RecordID = @RecordID
				end	
				select @InFlag = 0
			end		
		end		

	SET @i = @i + 1

	END
	
	-- delete processed records
	DELETE @tbl_TimeRecord
	WHERE IsProcessed=1

	-- Non full TimeIn/TimeOut
	declare cs cursor for 
	select RecordID, EmpID, RecordTime, InOutType 
	from @tbl_TimeRecord 
	order by EmpID,RecordTime
	open cs
	fetch next from cs into @RecordID, @EmpIDCur, @RecordTime, @InOutType
	while @@fetch_status = 0
	begin
		if @InOutType = 0 	
			insert @tbl_InOut(EmpID, TimeIn) values (@EmpIDCur, @RecordTime)
		else if (@InOutType = 1)
 			insert @tbl_InOut(EmpID, TimeOut) values (@EmpIDCur, @RecordTime)		
		delete @tbl_TimeRecord where RecordID = @RecordID
		fetch next from cs into @RecordID, @EmpIDCur, @RecordTime, @InOutType
	end
	close cs
	deallocate cs

	return
end
GO

CREATE OR ALTER function [dbo].[fnc_LeaveHalfDayID]  ()
returns int
as
BEGIN
	DECLARE	@LeaveHalfDayID int 
	select @LeaveHalfDayID = id
	from dbo.LeaveType 
	where Code = 'HAL'
--	print @LeaveHalfDayID -- leave half day need processing specially
	RETURN @LeaveHalfDayID
END
GO

CREATE OR ALTER function [dbo].[fnc_strSplit] ( @string nvarchar( 4000), @splitter char( 1) )
returns @res table ( id int primary key, rank int, val nvarchar( 4000) )
as
begin
     if substring ( @string, len ( @string), 1)<>@splitter
          set @string= @string+@splitter
     declare @start int, @word nvarchar(4000), @charindex int, @i int
     set @i=1
     set @start=1
     set @charindex= charindex( @splitter, @string, @start)
     while (@charindex <> 0)begin
          set @word= substring( @string, @start, @charindex - @start)
          set @start= @charindex +1
          set @charindex= charindex( @splitter, @string, @start)
          insert into @res  values ( @start, @i, LTrim(Rtrim(@word)))
          set @i=@i+1
     end
     return
end
GO

CREATE OR ALTER FUNCTION [dbo].[fnc_WorkingTimeRequest_GetRequestByDate_IsOutOfRange](@fromDate1 datetime, @toDate1 datetime, @fromDate2 datetime, @toDate2 datetime)
RETURNS bit
AS
BEGIN
	IF @toDate1 IS NULL
		SET @toDate1 = @fromDate1
	
	DECLARE @ret bit;
	SET @ret  = 0;
	if ( (@fromDate1 > @toDate2) or (@toDate1 < @fromDate2) )
		set @ret = 1;

	RETURN @ret;
END;
GO

CREATE OR ALTER FUNCTION [dbo].[RelatedDepartment] (
	@DepartmentId INT
	)
RETURNS @Departments TABLE (
	DepartmentId INT,
	DepartmentLevel INT
	)
AS
/*
Created: 20140325@Quinn.Vo
Task: Split string by character.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	INSERT INTO @Departments (DepartmentId, DepartmentLevel)
	SELECT DepartmentId, DepartmentLevel
	FROM dbo.Department WITH (NOLOCK)
	WHERE ParentId = @DepartmentId

	-- Add new child records until exhausted.
	WHILE @@ROWCOUNT > 0
	INSERT INTO @Departments (DepartmentId, DepartmentLevel)
	SELECT dp.DepartmentId, dp.DepartmentLevel
	FROM dbo.Department AS dp
	INNER JOIN @Departments AS d ON dp.ParentID = d.DepartmentId
	WHERE NOT EXISTS(
		SELECT 1 FROM @Departments AS CurrentRecords 
		WHERE CurrentRecords.DepartmentId= dp.DepartmentId
		)

	RETURN
END
GO

CREATE OR ALTER FUNCTION [dbo].[StrSplit](@string [nvarchar](MAX), @splitter [char](1))
RETURNS @res TABLE (
	[id] [int] NOT NULL,
	[rank] [int] NULL,
	[val] [nvarchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)
) WITH EXECUTE AS CALLER
AS 
begin
     if substring ( @string, len ( @string), 1)<>@splitter
          set @string= @string+@splitter
     declare @start int, @word nvarchar(max), @charindex int, @i int
     set @i=1
     set @start=1
     set @charindex= charindex( @splitter, @string, @start)
     while (@charindex <> 0)begin
          set @word= substring( @string, @start, @charindex - @start)
          set @start= @charindex +1
          set @charindex= charindex( @splitter, @string, @start)
          insert into @res  values ( @start, @i, @word)
          set @i=@i+1
     end
     return
end
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_DeleteTransaction] @LeaveTransactionId INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/20@Dan
Task: soft delete leave transaction and calculate annual leave balance for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
    SET XACT_ABORT ON;

    DECLARE @OldDeductDays NUMERIC(18, 1)
        ,@OldCreditDays NUMERIC(18, 1)
        ,@EmpID INT
        ,@isDeleted BIT

    SELECT @EmpID = EmpID
        ,@OldDeductDays = ISNULL(DeductDays, 0)
        ,@OldCreditDays = ISNULL(CreditDays, 0)
        ,@isDeleted = Deleted
    FROM dbo.LeaveTrans WITH (NOLOCK)
    WHERE Id = @LeaveTransactionId

    IF (ISNULL(@isDeleted, 0) = 1)
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Delete leave transaction unsuccessfully.'

        RETURN
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.LeaveTrans WITH (ROWLOCK, UPDLOCK)
        SET Deleted = 1
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
            ,ModifiedBy = @ModifiedBy
        WHERE Id = @LeaveTransactionId

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Delete leave transaction unsuccessfully.'
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_GetByID] @LeaveTransactionId INT
AS
/*
Created: 20160223@Quinn.Vo
Task: Get leave transaction by id.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT t.ID AS LeaveTransactionId
        ,RequestID
        ,FromDate AS AffectedFrom
        ,ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0) AS [ToTalDays]
        ,Description
        ,t.CreatedDate
        ,t.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.LeaveTrans AS t WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.ID = t.EmpID
    WHERE t.[Id] = @LeaveTransactionId
        AND RequestId IS NULL
        AND ISNULL(Deleted, 0) = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_GetEmpTransByMonth] @EmpId INT
    ,@Year INT
    ,@Month INT
AS
/*
Created: 2012/05/29@Dan
Task: show annual leave tranasaction by employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT t.ID AS LeaveTransactionId
        ,RequestID
        ,FromDate AS AffectedFrom
		,ToDate
        ,ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0) AS [ToTalDays]
        ,Description
        ,t.CreatedDate
        ,t.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.LeaveTrans AS t WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.ID = t.EmpID
    WHERE EmpId = @EmpId
        AND YEAR(fromdate) = @Year
		AND MONTH(fromdate) = @Month
        AND ISNULL(Deleted, 0) = 0
	ORDER BY FromDate DESC
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_GetOpenAndCloseBalance] @OpenDate DATETIME
	,@CloseDate DATETIME
AS
/*
Created: 20141229@Quinn.Vo
Task: Get current is open and close balance. 
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsClosedBalance BIT
		,@IsOpenedBalance BIT

	SET @IsClosedBalance = 0
	SET @IsOpenedBalance = 0

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.LeaveTrans WITH (NOLOCK)
				WHERE FromDate = @OpenDate
					AND IsOpenBalance = 1
				)
			)
	BEGIN
		SET @IsOpenedBalance = 1
	END

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.LeaveTrans WITH (NOLOCK)
				WHERE FromDate = @CloseDate
					AND IsCloseBalance = 1
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
				INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON wk.EmpId = e.Id AND e.isDeleted = 0
				WHERE YEAR(FromDate) = YEAR(@CloseDate)
					AND RequestType = 6
					AND LeaveTypeId IN (2, 30) 
					AND [Status] = 1
				)
			)
	BEGIN
		SET @IsClosedBalance = 1
	END

	SELECT @IsOpenedBalance AS IsOpenedBalance
		,@IsClosedBalance AS IsClosedBalance
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_GetSummary] @year INT
    ,@employeeId INT = NULL
    ,@groupId INT = NULL
    ,@departmentId INT = NULL
    ,@minAnnualBalance INT = 0
AS
/*
Created: 20160222@Quinn.Vo
Task: Get annual leave summary.
DB: NexHR
Original: 
Revisions:
    - 20250516@Henry.Dinh: add department id filter
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    SELECT e.Id AS EmpId
        ,SUM(ISNULL(CreditDays, 0)) CreditDays
        ,SUM(ISNULL(DeductDays, 0)) DeductDays
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 1 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jan
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 2 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Feb
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 3 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Mar
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 4 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Apr
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 5 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) May
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 6 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jun
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 7 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jul
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 8 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Aug
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 9 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Sep
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 10 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Oct
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 11 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Nov
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 12 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) DEC
    INTO #CurrenBalace
    FROM Employee e WITH (NOLOCK)
        LEFT JOIN (
        SELECT EmpId
            ,CreditDays
            ,DeductDays
            ,FromDate
        FROM dbo.LeaveTrans WITH (NOLOCK)
        WHERE ISNULL(Deleted, 0) = 0
            AND YEAR(FromDate) = @year
        ) t ON e.Id = t.Empid
    WHERE e.IsSpecialUser = 0
		AND (
            @employeeId IS NULL
            OR e.id = @employeeId
            )
        AND (
            @groupId IS NULL
            OR e.groupId = @groupId
            )
        AND (
            @departmentId IS NULL
            OR e.groupId IN (SELECT Id FROM EmployeeGroup WHERE DepartmentId = @departmentId)
            )
        AND (ISNULL(e.isDeleted, 0) = 0)
    GROUP BY e.ID

    SELECT 
        ISNULL(CreditDays, 0) [AnnualLeaveCurrentYear]
        ,Jan
        ,Feb
        ,Mar
        ,Apr
        ,May
        ,Jun
        ,Jul
        ,Aug
        ,Sep
        ,Oct
        ,Nov
        ,DEC
        ,ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0) AS LeaveBalance
        ,cb.EmpId
        ,e.Code
        ,FullName
        ,NickName
        ,d.DepartmentId
        ,d.Name AS DepartmentName
        ,e.GroupId AS TeamId
        ,g.Code AS TeamCode
        ,g.NAME AS TeamName
    FROM #CurrenBalace cb
        INNER JOIN Employee e WITH (NOLOCK) ON e.Id = cb.Empid
        INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID = e.GroupId
        INNER JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
     WHERE 
		e.IsSpecialUser = 0
    ORDER BY d.Name
        ,g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate

    DROP TABLE #CurrenBalace
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_GetTransByEmp] @EmpId INT
    ,@Year INT
    ,@excludedRequestTrans BIT
AS
/*
Created: 2012/05/29@Dan
Task: show annual leave tranasaction by employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT t.ID AS LeaveTransactionId
        ,RequestID
        ,FromDate AS AffectedFrom
		,ToDate
        ,ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0) AS [ToTalDays]
        ,Description
        ,t.CreatedDate
        ,t.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.LeaveTrans AS t WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.ID = t.EmpID
    WHERE EmpId = @EmpId
        AND YEAR(fromdate) = @Year
        AND ISNULL(Deleted, 0) = 0
        AND (
            @excludedRequestTrans = 0
            OR RequestId IS NULL
            )
	ORDER BY FromDate DESC
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_InsertMultipleTransactions] @EmpIds NVARCHAR(4000) = NULL
    ,@Description NVARCHAR(4000) = NULL
    ,@FromDate DATETIME
    ,@DeductDays NUMERIC(18, 1)
    ,@CreditDays NUMERIC(18, 1)
    ,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/17@Dan
Task: record multiple annual leave transaction and calculate annual leave balance for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON
    SET XACT_ABORT ON;

    DECLARE @Emp TABLE (ID INT)

    INSERT INTO @Emp (ID)
    SELECT val
    FROM dbo.[fnc_strSplit](@EmpIDs, ',')

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.LeaveTrans (
            EmpId
            ,Description
            ,FromDate
            ,DeductDays
            ,CreditDays
            ,CreatedDate
            ,CreatedBy
            )
        SELECT ID
            ,@Description
            ,@FromDate
            ,@DeductDays
            ,@CreditDays
            ,dbo.fnc_GetClientDateTime()
            ,@CreatedBy
        FROM Employee
        WHERE (
                (
                    @EmpIds IS NULL
                    AND ISNULL(isdeleted, 0) = 0
                    )
                OR Id IN (
                    SELECT id
                    FROM @Emp
                    )
                )

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Add leave transaction unsuccessfully.'
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_InsertTransaction] @EmployeeId INT
    ,@RequestId INT = NULL
    ,@Description NVARCHAR(4000) = NULL
    ,@FromDate DATETIME
    ,@ToDate DATETIME = NULL
    ,@DeductDays NUMERIC(18, 1)
    ,@CreditDays NUMERIC(18, 1)
    ,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/17@Dan
Task: record leave transaction and calculate annual leave balance for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.LeaveTrans (
            EmpId
            ,RequestId
            ,Description
            ,FromDate
            ,ToDate
            ,DeductDays
            ,CreditDays
            ,CreatedDate
            ,CreatedBy
            )
        VALUES (
            @EmployeeId
            ,@RequestId
            ,@Description
            ,@FromDate
            ,@ToDate
            ,@DeductDays
            ,@CreditDays
            ,dbo.fnc_GetClientDateTime()
            ,@CreatedBy
            )

        --UPDATE dbo.Employee WITH (ROWLOCK, UPDLOCK)
        --SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance, 0) + ISNULL(@CreditDays, 0) - ISNULL(@DeductDays, 0)
        --WHERE Id = @EmployeeId

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Add leave transaction unsuccessfully.'
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_ResetALAndLateInEarlyOut] 
AS
BEGIN
	SET XACT_ABORT ON;
	DECLARE @Description NVARCHAR(4000) = NULL
	,@FromDate DATETIME = '2024-02-01'
	,@ResetDays NUMERIC(18, 1)
	,@CreatedBy INT
	,@ErrCode INT = 0
	,@ErrMsg VARCHAR(200) = ''

	DECLARE @i INT
		,@TempEmpId INT
		,@TempLeaveBalance NUMERIC(18, 1)
		,@xml XML
	DECLARE @Emp TABLE (
		Id INT IDENTITY(1, 1)
		,EmpId INT
		,LeaveBalance FLOAT
		)

	INSERT INTO @Emp (
		EmpId
		,LeaveBalance
		)
	SELECT a.EmpId, (a.CreditDays - b.DeductDays) Balance from 
(select EmpId, CreditDays from LeaveTrans where FromDate = '2024-01-01' AND ToDate = '2024-01-31')
as a 
INNER JOIN dbo.Employee AS e ON a.EmpId = e.Id AND e.IsSpecialUser = 0 AND e.NickName NOT IN ('Sean', 'Joe', 'Daniel')
LEFT JOIN
(select EmpId, SUM(DeductDays) AS DeductDays from LeaveTrans where ISNULL(Deleted,0) = 0 AND FromDate >= '2024-01-01' AND (ToDate IS NULL OR ToDate <= '2024-01-31')
GROUP BY EmpId) AS b ON a.EmpId = b.EmpId
WHERE a.CreditDays - b.DeductDays > 0
AND e.NickName = 'Bill'

	BEGIN TRY
		BEGIN TRANSACTION;

		SET @i = 1

		WHILE EXISTS (
				SELECT 1
				FROM @Emp
				WHERE Id = @i
				)
		BEGIN
			SELECT @TempEmpId = EmpId
				,@TempLeaveBalance = LeaveBalance
			FROM @Emp
			WHERE Id = @i

			IF (
					EXISTS (
						SELECT 1
						FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
						WHERE EmpId = @TempEmpId
							AND [Year] = YEAR(@FromDate)
							AND [Minutes] > 0
						)
					)
			BEGIN
				EXEC [dbo].[LeaveDeduction_Insert] @TempEmpId
					,'Deduct 0.5 day to reset annual leave and late in/early out'
					,@FromDate
					,0.5
					,1
					,125
					,@ErrCode
					,@ErrMsg
				
				SET @TempLeaveBalance = @TempLeaveBalance - 0.5
				IF @TempLeaveBalance > 0
				BEGIN
					INSERT INTO dbo.LeaveTrans (
						EmpId
						,Description
						,FromDate
						,DeductDays
						,CreditDays
						,CreatedDate
						,CreatedBy
						,IsCloseBalance
						)
					SELECT @TempEmpId
						,'Close unused AL balance of 2023'
						,@FromDate
						,@TempLeaveBalance
						,0
						,dbo.fnc_GetClientDateTime()
						,@CreatedBy
						,1
				END
			END
			ELSE
			BEGIN
				INSERT INTO dbo.LeaveTrans (
					EmpId
					,Description
					,FromDate
					,DeductDays
					,CreditDays
					,CreatedDate
					,CreatedBy
					,IsCloseBalance
					)
				SELECT @TempEmpId
					,'Close unused AL balance of 2023'
					,@FromDate
					,@TempLeaveBalance
					,1
					,dbo.fnc_GetClientDateTime()
					,@CreatedBy
					,1
			END

			SET @i = @i + 1
		END

		UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
		SET [Minutes] = 0
		WHERE [Minutes] < 0
			AND [Year] = YEAR(@FromDate)

		CREATE TABLE #EmpBanlance (
			EmpId INT
			,LateInEarlyOut INT
			,NexBalance INT
			)

		INSERT INTO #EmpBanlance (
			EmpId
			,LateInEarlyOut
			,NexBalance
			)
		SELECT la.EmpId
			,CASE 
				WHEN (la.Minutes - ISNULL(ot.Minutes, 0)) < 0 THEN 0
				ELSE (la.Minutes - ISNULL(ot.Minutes, 0))
				END AS Late
			,CASE 
				WHEN (la.Minutes - ISNULL(ot.Minutes, 0)) > 0 THEN 0
				ELSE (ISNULL(ot.Minutes, 0) - la.Minutes)
				END AS OT
		FROM (
			SELECT *
			FROM dbo.[LateInEarlyOutYearly] WITH (NOLOCK)
			WHERE [Year] = YEAR(@FromDate)
			) AS la
			LEFT JOIN (
			SELECT *
			FROM dbo.OvertimeYearly WITH (NOLOCK)
			WHERE [Year] = YEAR(@FromDate)
			) AS ot ON la.EmpId = ot.EmpId

		INSERT INTO [dbo].[LateInEarlyOutYearly] (
			[EmpId]
			,[Year]
			,[Minutes]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT EmpId
			,YEAR(@FromDate) + 1
			,LateInEarlyOut
			,@CreatedBy
			,dbo.fnc_GetClientDateTime()
		FROM #EmpBanlance AS bl
		WHERE NOT EXISTS (SELECT 1 FROM [dbo].[LateInEarlyOutYearly] WITH (NOLOCK) WHERE bl.EmpId = EmpId AND [Year] = YEAR(@FromDate) + 1)

		INSERT INTO [dbo].[OvertimeYearly] (
			[EmpId]
			,[Year]
			,[Minutes]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT EmpId
			,YEAR(@FromDate) + 1
			,NexBalance
			,@CreatedBy
			,dbo.fnc_GetClientDateTime()
		FROM #EmpBanlance AS bl
		WHERE NOT EXISTS (SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK) WHERE bl.EmpId = EmpId AND [Year] = YEAR(@FromDate) + 1)

		DROP TABLE #EmpBanlance

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Reset leave transaction unsuccessfully.'
	END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_ResetMultipleTransactions] @EmpValue NVARCHAR(MAX) = NULL
	,@Description NVARCHAR(4000) = NULL
	,@FromDate DATETIME
	,@ResetDays NUMERIC(18, 1)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/12/24@Edward
Task: reset anual balance
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
    @EmpValue:
        <ROOT><Employee Id="1" LeaveBalance="2"/></ROOT>
*/
BEGIN
	SET XACT_ABORT ON;

	DECLARE @i INT
		,@TempEmpId INT
		,@TempLeaveBalance NUMERIC(18, 1)
		,@xml XML
	DECLARE @Emp TABLE (
		Id INT IDENTITY(1, 1)
		,EmpId INT
		,LeaveBalance FLOAT
		)

	SET @xml = CONVERT(XML, @EmpValue)

	INSERT INTO @Emp (
		EmpId
		,LeaveBalance
		)
	SELECT T.C.value('./@Id', 'int')
		,T.C.value('./@LeaveBalance', 'numeric(18,1)')
	FROM @xml.nodes('/ROOT/Employee') AS [T](C) --T: table, C: column

	BEGIN TRY
		BEGIN TRANSACTION;

		SET @i = 1

		WHILE EXISTS (
				SELECT 1
				FROM @Emp
				WHERE Id = @i
				)
		BEGIN
			SELECT @TempEmpId = EmpId
				,@TempLeaveBalance = LeaveBalance
			FROM @Emp
			WHERE Id = @i

			IF (
					EXISTS (
						SELECT 1
						FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
						WHERE EmpId = @TempEmpId
							AND [Year] = YEAR(@FromDate)
							AND [Minutes] > 0
						)
					)
			BEGIN
				EXEC [dbo].[LeaveDeduction_Insert] @TempEmpId
					,@Description
					,@FromDate
					,@TempLeaveBalance
					,1
					,@CreatedBy
					,@ErrCode
					,@ErrMsg
			END
			ELSE
			BEGIN
				INSERT INTO dbo.LeaveTrans (
					EmpId
					,Description
					,FromDate
					,DeductDays
					,CreditDays
					,CreatedDate
					,CreatedBy
					,IsCloseBalance
					)
				SELECT @TempEmpId
					,@Description
					,@FromDate
					,@TempLeaveBalance
					,0
					,dbo.fnc_GetClientDateTime()
					,@CreatedBy
					,1

			END

			SET @i = @i + 1
		END

		UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
		SET [Minutes] = 0
		WHERE [Minutes] < 0
			AND [Year] = YEAR(@FromDate)

		CREATE TABLE #EmpBanlance (
			EmpId INT
			,LateInEarlyOut INT
			,NexBalance INT
			)

		INSERT INTO #EmpBanlance (
			EmpId
			,LateInEarlyOut
			,NexBalance
			)
		SELECT la.EmpId
			,CASE 
				WHEN (la.Minutes - ISNULL(ot.Minutes, 0)) < 0 THEN 0
				ELSE (la.Minutes - ISNULL(ot.Minutes, 0))
				END AS Late
			,CASE 
				WHEN (la.Minutes - ISNULL(ot.Minutes, 0)) > 0 THEN 0
				ELSE (ISNULL(ot.Minutes, 0) - la.Minutes)
				END AS OT
		FROM (
			SELECT *
			FROM dbo.[LateInEarlyOutYearly] WITH (NOLOCK)
			WHERE [Year] = YEAR(@FromDate)
			) AS la
			LEFT JOIN (
			SELECT *
			FROM dbo.OvertimeYearly WITH (NOLOCK)
			WHERE [Year] = YEAR(@FromDate)
			) AS ot ON la.EmpId = ot.EmpId

		INSERT INTO [dbo].[LateInEarlyOutYearly] (
			[EmpId]
			,[Year]
			,[Minutes]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT EmpId
			,YEAR(@FromDate) + 1
			,LateInEarlyOut
			,@CreatedBy
			,dbo.fnc_GetClientDateTime()
		FROM #EmpBanlance AS bl
		WHERE NOT EXISTS (SELECT 1 FROM [dbo].[LateInEarlyOutYearly] WITH (NOLOCK) WHERE bl.EmpId = EmpId AND [Year] = YEAR(@FromDate) + 1)

		INSERT INTO [dbo].[OvertimeYearly] (
			[EmpId]
			,[Year]
			,[Minutes]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT EmpId
			,YEAR(@FromDate) + 1
			,NexBalance
			,@CreatedBy
			,dbo.fnc_GetClientDateTime()
		FROM #EmpBanlance AS bl
		WHERE NOT EXISTS (SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK) WHERE bl.EmpId = EmpId AND [Year] = YEAR(@FromDate) + 1)

		DROP TABLE #EmpBanlance

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Reset leave transaction unsuccessfully.'
	END CATCH;
END

GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_TransferBalance] 
AS
BEGIN
	DECLARE @FromDate DATETIME = '2023-12-31',
		@CreatedBy INT = 125

		--CREATE TABLE #EmpBanlance (
		--	EmpId INT
		--	,LateInEarlyOut INT
		--	,NexBalance INT
		--	)

		--INSERT INTO #EmpBanlance (
		--	EmpId
		--	,LateInEarlyOut
		--	,NexBalance
		--	)
		--SELECT la.EmpId
		--	,CASE 
		--		WHEN (la.Minutes - ISNULL(ot.Minutes, 0)) < 0 THEN 0
		--		ELSE (la.Minutes - ISNULL(ot.Minutes, 0))
		--		END AS Late
		--	,CASE 
		--		WHEN (la.Minutes - ISNULL(ot.Minutes, 0)) > 0 THEN 0
		--		ELSE (ISNULL(ot.Minutes, 0) - la.Minutes)
		--		END AS OT
		--FROM (
		--	SELECT *
		--	FROM dbo.[LateInEarlyOutYearly] WITH (NOLOCK)
		--	WHERE [Year] = YEAR(@FromDate)
		--	) AS la
		--	LEFT JOIN (
		--	SELECT *
		--	FROM dbo.OvertimeYearly WITH (NOLOCK)
		--	WHERE [Year] = YEAR(@FromDate)
		--	) AS ot ON la.EmpId = ot.EmpId

		--INSERT INTO [dbo].[LateInEarlyOutYearly] (
		--	[EmpId]
		--	,[Year]
		--	,[Minutes]
		--	,[ModifiedBy]
		--	,[ModifiedDate]
		--	)
		--SELECT EmpId
		--	,YEAR(@FromDate) + 1
		--	,LateInEarlyOut
		--	,@CreatedBy
		--	,dbo.fnc_GetClientDateTime()
		--FROM #EmpBanlance AS bl
		--WHERE NOT EXISTS (
		--SELECT 1 
		--FROM [dbo].[LateInEarlyOutYearly] WITH (NOLOCK) 
		--WHERE bl.EmpId = EmpId AND [Year] = (YEAR(@FromDate) + 1))

		--INSERT INTO [dbo].[OvertimeYearly] (
		--	[EmpId]
		--	,[Year]
		--	,[Minutes]
		--	,[ModifiedBy]
		--	,[ModifiedDate]
		--	)
		--SELECT EmpId
		--	,YEAR(@FromDate) + 1
		--	,NexBalance
		--	,@CreatedBy
		--	,dbo.fnc_GetClientDateTime()
		--FROM #EmpBanlance AS bl
		--WHERE NOT EXISTS (
		--SELECT 1 
		--FROM [dbo].[OvertimeYearly] WITH (NOLOCK) 
		--WHERE bl.EmpId = EmpId AND [Year] = (YEAR(@FromDate) + 1))

		--DROP TABLE #EmpBanlance

		CREATE TABLE #EmpLeaveBanlance (
			EmpId INT
			,Balance NUMERIC(18,1)
			)

	INSERT INTO #EmpLeaveBanlance(EmpId, Balance)
	SELECT EmpId, (SUM(CreditDays) - SUM(DeductDays)) AS AnnualLeaveBalance
	FROM dbo.LeaveTrans as a WITH (NOLOCK)
	INNER JOIN dbo.Employee AS b WITH (NOLOCK) ON a.EmpId = b.Id AND b.isDeleted = 0
	WHERE 
		 ISNULL(Deleted, 0) = 0  
		AND YEAR(FromDate) = YEAR(@FromDate)
	GROUP BY EmpId
	HAVING (SUM(CreditDays) - SUM(DeductDays)) > 0

	
	INSERT INTO dbo.LeaveTrans(
		 [EmpId]
      ,[Description]
      ,[FromDate]
      ,[DeductDays]
      ,[CreditDays]
      ,[CreatedDate]
      ,[CreatedBy]
	)
	SELECT EmpId,
		'Transfer Balance to 2024',
		@FromDate,
		Balance,
		0,
		dbo.fnc_GetClientDateTime(),
		@CreatedBy
	FROM #EmpLeaveBanlance

	INSERT INTO dbo.LeaveTrans(
		 [EmpId]
      ,[Description]
      ,[FromDate]
	  ,[ToDate]
      ,[DeductDays]
      ,[CreditDays]
      ,[CreatedDate]
      ,[CreatedBy]
	)
	SELECT EmpId,
		'Transfer Balance from 2023',
		'2024-01-01',
		'2024-01-31',
		0,
		Balance,
		dbo.fnc_GetClientDateTime(),
		@CreatedBy
	FROM #EmpLeaveBanlance

	DROP TABLE #EmpLeaveBanlance
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AnnualLeave_UpdateTransaction] @LeaveTransactionId INT
    ,@Description NVARCHAR(4000) = NULL
    ,@FromDate DATETIME
    ,@DeductDays NUMERIC(18, 1)
    ,@CreditDays NUMERIC(18, 1)
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/20@Dan
Task: update leave transaction and calculate annual leave balance for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
    SET XACT_ABORT ON;

    DECLARE @OldDeductDays NUMERIC(18, 1)
        ,@OldCreditDays NUMERIC(18, 1)
        ,@EmpID INT

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @EmpID = EmpID
            ,@OldDeductDays = ISNULL(DeductDays, 0)
            ,@OldCreditDays = ISNULL(CreditDays, 0)
        FROM dbo.LeaveTrans
        WHERE Id = @LeaveTransactionId

        UPDATE dbo.LeaveTrans WITH (ROWLOCK, UPDLOCK)
        SET Description = @Description
            ,FromDate = @FromDate
            ,ToDate = @FromDate
            ,DeductDays = @DeductDays
            ,CreditDays = @CreditDays
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
            ,ModifiedBy = @ModifiedBy
        WHERE Id = @LeaveTransactionId

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update leave transaction unsuccessfully.'
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[CheckInCheckOut_GetByConversationId] 
	@ConversationId VARCHAR(255)
AS
/*
Created: 20240719@Oliver
Task: get missing checkin today and checkout yesterday by conversationId
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
     CREATE TABLE #EmployeeAttendance
     (EmpId          INT,
      AttendanceDate DATETIME,
      MissingType    TINYINT
     );
     DECLARE @today DATETIME, @yesterday DATETIME, @now DATETIME, @startday DATETIME, @empId INT;
     SET @now = DATEADD(hh, 7, GETUTCDATE());
     SET @today = CONVERT(VARCHAR(10), @now, 111); --use UTC date because the server time is GMT -4, not GMT+7
     SET @yesterday = DATEADD(d, -1, @today);

	 SELECT @empId = c.EmpId
	 FROM dbo.EmployeeConversation c with (nolock) where c.ConversationId = @ConversationId

	 --SET @empId = 363
     
     IF(DATEPART(WEEKDAY, @today) = 2
        OR DATEPART(WEEKDAY, @today) = 3
        OR DATEPART(WEEKDAY, @today) = 4)
         BEGIN
             SET @startday = DATEADD(d, -5, @today);
         END;
     ELSE
         BEGIN
             SET @startday = DATEADD(d, -3, @today);
         END;
     
			BEGIN
				INSERT INTO #EmployeeAttendance
				(EmpId,
				AttendanceDate,
				MissingType
				)
					SELECT a.EmpId,
							AttendanceDate,
							1
					FROM dbo.EmployeeAttendance a WITH (NOLOCK)
							INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
																			AND c.WorkDate = AttendanceDate
						INNER JOIN dbo.Shift AS sh ON c.ShiftId = sh.Id
					WHERE attendancedate BETWEEN @startday AND @today
							--AND ISNULL(Absence, 0) = 0
							AND InFixed IS NULL
							AND dbo.fnc_GetClientDateTime() > c.WorkDate + ' ' + sh.EndCheckInTime
							AND a.EmpId = @EmpId
							AND isOffday = 0
							AND IsHoliday = 0
							AND (IsLeaveDay = 0
								OR (IsLeaveDay = 1
									AND a.LeaveDay = 0.5))
							AND NOT EXISTS
					(
						SELECT 1
						FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
						WHERE r.empid = a.empid 
								-- AND r.CreatedDate >= @yesterday 
								AND a.EmpId = @EmpId
								AND requesttype = 1
								AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
					)
							AND NOT EXISTS
					(
						SELECT 1
						FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
						WHERE r.empid = a.empid
								AND requesttype = 6
								AND a.EmpId = @EmpId
								AND r.TotalDays >= 1
								AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
								AND r.[Status] <> 3
					);
				INSERT INTO #EmployeeAttendance
				(EmpId,
				AttendanceDate,
				MissingType
				)
					SELECT a.EmpId,
							AttendanceDate,
							2
					FROM dbo.EmployeeAttendance a WITH (NOLOCK)
							INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
																			AND c.WorkDate = AttendanceDate
						INNER JOIN dbo.Shift AS sh ON c.ShiftId = sh.Id
					WHERE attendancedate BETWEEN @startday AND @yesterday
							AND a.EmpId = @EmpId
							AND dbo.fnc_GetClientDateTime() > c.WorkDate + ' ' + sh.EndCheckOutTime
							--AND ISNULL(Absence, 0) = 0
							AND OutFixed IS NULL
							AND isOffday = 0
							AND IsHoliday = 0
							AND (IsLeaveDay = 0
								OR (IsLeaveDay = 1
									AND a.LeaveDay = 0.5))
							AND NOT EXISTS
					(
						SELECT 1
						FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
						WHERE r.empid = a.empid 
								-- AND r.CreatedDate >= @yesterday 
								AND requesttype = 2
								AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
					)
							AND NOT EXISTS
					(
						SELECT 1
						FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
						WHERE r.empid = a.empid
								AND requesttype = 6
								AND r.TotalDays >= 1
								AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
								AND r.[Status] <> 3
					);

				IF EXISTS (SELECT 1 FROM #EmployeeAttendance)
				BEGIN
					SELECT a.EmpId,
						AttendanceDate,
						s.Code ShiftCode,
						a.MissingType,
						s.BeginCheckInTime,
						s.EndCheckOutTime
					FROM #EmployeeAttendance a
						INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empID = a.EmpID
																			AND c.WorkDate = AttendanceDate
						LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ID = c.ShiftId
					ORDER BY a.AttendanceDate DESC
				END
				ELSE
				BEGIN
					DECLARE @attendance datetime = dbo.fnc_GetClientDateTime()
					IF (DATEPART(WEEKDAY, @attendance) = 1 OR DATEPART(WEEKDAY, @attendance) = 7)
					BEGIN
						SET @attendance = DATEADD(DAY, 
						(CASE WHEN DATEPART(WEEKDAY, @attendance) = 1 THEN -2 ELSE -1 END ),
						@attendance)
					END

					SELECT TOP 1 c.EmpId,
								cast(@attendance as date) as AttendanceDate,
								s.Code ShiftCode,
								0 as MissingType,
								s.BeginCheckInTime,
								s.EndCheckOutTime
					FROM dbo.EmployeeCalendar c WITH (NOLOCK)
					LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ID = c.ShiftId
					WHERE CAST(c.WorkDate as DATE) = CAST(@attendance as DATE)
					AND c.EmpId = @empId
				END

			DROP TABLE #EmployeeAttendance;
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[CompanyEvent_Delete] @Id INT,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160419@Harvey.Nguyen
Task: Update event
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.CompanyEvent e WITH (NOLOCK)
				WHERE e.Id = @Id
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Can not find event for this Id !'

		RETURN
	END
	
	DELETE FROM dbo.CompanyEvent WHERE Id = @Id
	
	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[CompanyEvent_GetAll] @FromDate DATE
	,@ToDate DATE
AS
/*
Created: 20160419@Harvey.Nguyen
Task: Get all events
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT e.Id
		,e.Title
		,e.FromDate
		,(CASE 
			WHEN (e.FromDate = e.ToDate) THEN e.ToDate
			ELSE CASE 
					WHEN (
							e.FromTime = ''
							AND e.ToTime = ''
							) THEN DATEADD(day, 1, e.ToDate)
					ELSE e.ToDate
					END END
			) AS 'ToDate'
		,e.FromTime
		,e.ToTime
		,e.Location
		,e.Description
		,e2.NickName AS 'CreatedBy'
		,e.CreatedOn
	FROM dbo.CompanyEvent e WITH (NOLOCK)
		INNER JOIN dbo.Employee e2 WITH (NOLOCK) ON e.CreatedBy = e2.Id
	WHERE (
			@FromDate IS NULL
			OR e.FromDate >= @FromDate
			)
		AND (
			@ToDate IS NULL
			OR e.ToDate <= @ToDate
			)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[CompanyEvent_GetEventById] @Id NVARCHAR(150)
AS
/*
Created: 20160419@Harvey.Nguyen
Task: Get all events
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT e.Id
		,e.Title
		,e.FromDate
		,e.ToDate
		,e.FromTime
		,e.ToTime
		,e.Location
		,e.Description
		,e.CreatedBy
		,e.CreatedOn
	FROM dbo.CompanyEvent e WITH (NOLOCK)
	WHERE e.Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[CompanyEvent_GetEventInWeek]
AS
/*
Created: 20160512@Harvey.Nguyen
Task: Show event on Dashboard
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @now DATE = dbo.fnc_GetClientDateTime()
		,@startWeek DATE
		,@endWeek DATE

	SELECT @startWeek = DATEADD(dd, - (DATEPART(dw, @now) - 2), @now)

	SELECT @endWeek = DATEADD(dd, 15 - (DATEPART(dw, @now)), @now)

	SELECT e.Id
		,e.Title
		,e.FromDate
		,e.ToDate
		,e.FromTime
		,e.ToTime
		,e.Location
		,e.Description
	FROM dbo.CompanyEvent e WITH (NOLOCK)
	WHERE ((
			e.FromDate >= @startWeek
			AND e.FromDate <= @endWeek
			)
		OR (
			@startWeek >= e.FromDate
			AND @startWeek <= e.ToDate
			))
		AND (
			e.ToDate >= @now
		)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[CompanyEvent_Insert] @Title NVARCHAR(150)
	,@FromDate DATE
	,@ToDate DATE
	,@FromTime varchar(8)
	,@ToTime varchar(8)
	,@Location nvarchar(100)
	,@Description NVARCHAR(max)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160419@Harvey.Nguyen
Task: Insert new event
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.CompanyEvent e WITH (NOLOCK)
				WHERE FromDate = @FromDate
					AND e.ToDate = @ToDate
					AND e.Title = @Title
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'There are same event in date range.'

		RETURN
	END

	INSERT INTO dbo.CompanyEvent (
		Title
		,FromDate
		,ToDate
		,FromTime
		,ToTime
		,Location
		,Description
		,CreatedBy
		,CreatedOn
		)
	VALUES (
		@Title
		,@FromDate
		,@ToDate
		,@FromTime
		,@ToTime
		,@Location
		,@Description
		,@CreatedBy
		,dbo.fnc_GetClientDateTime()
		)

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[CompanyEvent_Update] @Id INT
	,@Title NVARCHAR(150)
	,@FromDate DATE
	,@ToDate DATE
	,@FromTime VARCHAR(8)
	,@ToTime VARCHAR(8)
	,@Location nvarchar(100)
	,@Description NVARCHAR(max)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160419@Harvey.Nguyen
Task: Update event
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.CompanyEvent e WITH (NOLOCK)
				WHERE e.Id = @Id
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Can not find event for this Id'

		RETURN
	END

	UPDATE dbo.CompanyEvent WITH (ROWLOCK, UPDLOCK)
	SET Title = @Title
		,FromDate = @FromDate
		,ToDate = @ToDate
		,FromTime = @FromTime
		,Location = @Location
		,ToTime = @ToTime
		,Description = @Description
	WHERE Id = @Id

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Conversation_GetTeamHr]
AS
/*
	Created: 2023@Brian.nguyen
	Task: Get group conversations team human resources
	DB: NexHRM
	Revisions: 
		20250507@Henry.Dinh: update group id from 38 to 1104
	Original: 
*/
BEGIN
	SET NOCOUNT ON; 

	SELECT e.Id 
        ,e.NickName 
		,ec.ConversationId
		--,'29:1KmVXImsgANnLEyWFPGem8Y4K4fMFvYN-tE_QfuS0SfASNxMSmouUgyega3ebG6yZ' AS ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK)
			ON e.Id = ec.EmpId
    WHERE  e.IsSpecialUser = 0 
		AND e.isDeleted = 0
		AND e.Enabled = 1
		AND GroupId = 1104
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Dashboard_EmployeeSpace] @EmpId INT
AS
/*
Created: 20150817@Quinn.Vo
Task: Show dashboard info for employee.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
- Type: 1 Leave, 2: Check In/Out, 3: Missing Check In/Out, 4: Late In/Early Out, 5: Absent, 6: Total Working Hours, 7: Annual Leave Balance
*/
BEGIN
	DECLARE @CurrentMonth DATETIME
		,@LastMonth DATETIME
		,@Today DATETIME

	SET @Today = dbo.fnc_GetClientDateTime()

	SELECT @CurrentMonth = @Today
		,@LastMonth = DATEADD(M, - 1, @Today)

	SELECT wk.FromDate AS WorkDate
		,(CASE 
			WHEN DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0 THEN 1
			ELSE 0 END
			) AS IsCurrent
		,2 AS TypeId
		,0 AS LeaveDay
		,wk.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = wk.EmpId
	WHERE wk.EmpId = @EmpId
		AND wk.RequestType IN (1, 2)
		AND wk.[Status] IN (1, 2)
		AND DATEDIFF(M, @LastMonth, wk.FromDate) > = 0
		AND DATEDIFF(M, wk.FromDate, @CurrentMonth) > = 0
		AND EXISTS (
			SELECT 1 
			FROM dbo.EmployeeCalendar WITH (NOLOCK) 
			WHERE DATEDIFF(dd, WorkDate, wk.FromDate) = 0
			AND IsHoliday = 0
			AND IsOffDay = 0
		)
	
	UNION
	SELECT cl.WorkDate
		,(CASE 
			WHEN DATEDIFF(M, cl.WorkDate, @CurrentMonth) = 0 THEN 1
			ELSE 0 END
			) AS IsCurrent
		,1 AS TypeId
		,(CASE 
			WHEN wk.TotalDays = 0.5 THEN 0.5
			ELSE 1 END
			) AS LeaveDay
		,wk.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = wk.EmpId
		INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON DATEDIFF(D, wk.FromDate, cl.WorkDate) >= 0
		AND DATEDIFF(D, cl.WorkDate, wk.ToDate) >= 0
		AND wk.EmpId = cl.EmpId
		AND cl.IsOffDay <> 1
		AND cl.IsHoliday <> 1
	WHERE wk.EmpId = @EmpId
		AND wk.RequestType = 6
		AND wk.[Status] IN (1, 2)
		AND DATEDIFF(M, @LastMonth, cl.WorkDate) >= 0
		AND DATEDIFF(M, cl.WorkDate, @CurrentMonth) >= 0
	--GROUP BY wk.EmpId
	
	UNION
	SELECT cl.WorkDate
		,(CASE 
			WHEN DATEDIFF(M, cl.WorkDate, @CurrentMonth) = 0 THEN 1
			ELSE 0 END
			) AS IsCurrent
		,CASE 
			WHEN (
					(
						att.LateIn > 0
						OR att.EarlyOut > 0
						)
					-- AND e.Id NOT IN (88, 89, 123, 134, 153)
					) THEN 4
			ELSE CASE 
					WHEN (
							(
								ISNULL(Absence, 0) = 0
								AND att.InFixed IS NULL
								AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
								)
							OR (
								ISNULL(Absence, 0) = 0
								AND att.OutFixed IS NULL
								AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckOutTime), @Today) > 0
								)
							) THEN 3
					ELSE CASE 
							WHEN (
									ISNULL(att.Absence, 0) = 1
									AND cl.IsOffDay = 0
									AND cl.IsHoliday = 0
									AND ISNULL(att.LeaveDay, 0) <> 1
									AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
									) THEN 5
							ELSE 0
							END
					END
			END AS TypeId
		,0 AS LeaveDay
		,att.EmpId AS EmployeeId
		,e.NickName
		,e.FullName
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e ON att.EmpId = e.Id
		INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON cl.EmpId = att.EmpId
		AND cl.Workdate = att.AttendanceDate
		LEFT JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON cl.EmpId = wk.EmpId
		AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
		AND wk.RequestType IN (1, 2)
		AND wk.[Status] = 1
		AND (
			(
				att.InFixed IS NULL
				AND wk.RequestType = 1
				)
			OR (
				att.OutFixed IS NULL
				AND wk.RequestType = 2
				)
			)
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.Id = att.ShiftId
	WHERE att.EmpId = @EmpId
		AND DATEDIFF(M, @LastMonth, cl.WorkDate) >= 0
		AND DATEDIFF(M, cl.WorkDate, @CurrentMonth) >= 0
		AND (
			(
				(
					att.LateIn > 0
					OR att.EarlyOut > 0
					)
				-- AND e.Id NOT IN (88, 89, 123, 134, 153)
				)
			OR (
				ISNULL(att.Absence, 0) = 1
				AND cl.IsOffDay = 0
				AND cl.IsHoliday = 0
				AND ISNULL(att.LeaveDay, 0) <> 1
				AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
				AND wk.Id IS NULL
				)
			OR (
				ISNULL(Absence, 0) = 0
				AND att.InFixed IS NULL
				AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
				AND (
					wk.Id IS NULL
					OR wk.RequestType = 2
					)
				)
			OR (
				ISNULL(Absence, 0) = 0
				AND att.OutFixed IS NULL
				AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckOutTime), @Today) > 0
				AND (
					wk.Id IS NULL
					OR wk.RequestType = 1
					)
				)
			)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[DBA_AnnualLeave_CreateAnnualBalance] @Year INT
    ,@EmpID INT = NULL
    ,@Description NVARCHAR(1000) = ''
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@2014.12.26
Task: Add annual balance leave for new year.
DB: HRM_SMS
Original: 

Revisions:    
	- Oliver@2024.12.31: change to new rule of AL credit
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF LEN(ISNULL(@Description, '')) = 0
        SET @Description = 'Add annual leave days for ' + CONVERT(NVARCHAR(10), @Year) + '.'

    DECLARE @CreditDays NUMERIC(18, 1)
		,@EndDate DATETIME
        ,@FromDate DATETIME
        ,@ToDate DATETIME
        ,@CalDate_Annual DATETIME

    SELECT @FromDate = CONVERT(NVARCHAR(10), @Year) + '/01/01'
        ,@ToDate = CONVERT(NVARCHAR(10), @Year) + '/12/31'
        ,@CalDate_Annual = CONVERT(NVARCHAR(10), @Year) + '/07/01'
		,@EndDate =  CONVERT(NVARCHAR(10), @Year) + '/12/31'

    SET @CreditDays = 12

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.LeaveTrans (
            EmpId
            ,[Description]
            ,FromDate
            ,ToDate
            ,DeductDays
            ,CreditDays
            ,CreatedDate
            ,CreatedBy
            ,Deleted
            ,IsOpenBalance
            )
        SELECT e.Id
            ,@Description Description
            ,@FromDate FromDate
            ,@ToDate ToDate
            ,0 AS DeductDays
            ,@CreditDays - 1
				+ CASE 
					WHEN eg.DepartmentId IN (1, 3) 
					THEN (CASE 
						WHEN FLOOR(DATEDIFF(DAY, DATEADD(year, 1, e.GroupDate), @EndDate)/730) > 19 THEN 19 
						ELSE FLOOR(DATEDIFF(DAY, DATEADD(year, 1, e.GroupDate), @EndDate)/730) + 1
						END)
					ELSE (CASE 
						WHEN FLOOR(DATEDIFF(DAY, e.GroupDate, @EndDate)/365) > 19 THEN 19 
						ELSE FLOOR(DATEDIFF(DAY, e.GroupDate, @EndDate)/365) 
						END)
					END AS CreditDays
            ,dbo.fnc_GetClientDateTime()
            ,NULL CreatedBy
            ,0 Deleted
            ,1 as IsOpenBalance
        FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup eg WITH (NOLOCK) ON e.GroupId = eg.Id
            LEFT JOIN (
            SELECT MAX(FromDate) FromDate
                ,EmpId
            FROM dbo.LeaveTrans lt
            WHERE lt.FromDate >= @FromDate
                AND lt.CreditDays > 0
                AND ISNULL(Deleted, 0) = 0
            GROUP BY EmpId
            ) lt ON e.Id = lt.EmpId
        WHERE ISNULL(isdeleted, 0) = 0
            AND e.[Enabled] = 1
            AND e.GroupDate <= @FromDate
            AND e.ResignedDate IS NULL
            AND (
                e.Id = @EmpID
                OR @EmpID IS NULL
                )
            AND lt.EmpId IS NULL

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrCode = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION;

        SELECT @ErrCode = 100
            ,@ErrMsg = N'Add annual balance failed.'

        RETURN
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[DBA_AnnualLeave_DeleteAnnualBalance]
    @Year INT,
    @EmpID INT=NULL,
    @FromDate datetime,

    @Description NVARCHAR(1000)='Add annual leave days for',
    @ErrCode int = 0 OUTPUT,
    @ErrMsg varchar(200) = '' OUTPUT    
AS
/*
Created: Quinn.Vo@2014.12.26
Task: delete annual balance leave.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	DELETE dbo.LeaveTrans
        WHERE (EmpId = @EmpID OR @EmpID IS NULL) AND FromDate= @FromDate AND CreditDays > 0    AND [Description] = @Description AND RequestId IS NULL
END
GO

CREATE OR ALTER PROCEDURE [dbo].[DBA_AnnualLeave_TransferBalanceFromYear] @Year INT
    ,@EmpID INT = NULL
    ,@Description NVARCHAR(1000) = ''
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Oliver@2024.12.31
Task: Transfer AL unused from last year to current year
DB: NexHRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF LEN(ISNULL(@Description, '')) = 0
        SET @Description = 'Transfer AL from ' + CONVERT(NVARCHAR(10), @Year) + ''

    DECLARE @FromDate DATETIME
        ,@ToDate DATETIME

    SELECT @FromDate = CONVERT(NVARCHAR(10), @Year + 1) + '/01/01'
        ,@ToDate = CONVERT(NVARCHAR(10), @Year + 1) + '/03/31'

    BEGIN TRY
        BEGIN TRANSACTION;

		CREATE TABLE #EmployeeLastYearBalance
		 (EmpId          INT,
		  LastYearAL NUMERIC(18,1),
		  IsMaxAL BIT
		 );

		INSERT INTO #EmployeeLastYearBalance(EmpId, LastYearAL, IsMaxAL)
		SELECT e.Id
				,SUM(lt.CreditDays) - SUM(lt.DeductDays) as LastYearAL
				,(CASE 
				WHEN EXISTS (SELECT 1 FROM dbo.LeaveTrans with (nolock) where EmpId = e.Id and CreditDays >= 29 and YEAR(FromDate) = @Year + 1) 
				THEN 1 ELSE 0
			END) as IsMaxAL 
		FROM [dbo].[LeaveTrans] lt with (nolock)
		INNER JOIN dbo.Employee e with (nolock) on lt.EmpId = e.Id
		WHERE YEAR(lt.FromDate) = 2024 AND ISNULL(lt.Deleted, 0) = 0
			AND ISNULL(isdeleted, 0) = 0
			AND e.[Enabled] = 1
			AND (
				e.Id = @EmpID
				OR @EmpID IS NULL
				)
		GROUP BY lt.EmpId, e.Id, e.NickName

        INSERT INTO dbo.LeaveTrans (
            EmpId
            ,[Description]
            ,FromDate
            ,ToDate
            ,DeductDays
            ,CreditDays
            ,CreatedDate
            ,CreatedBy
            ,Deleted
            ,IsOpenBalance
            )
        SELECT e.EmpId
            ,@Description Description
            ,@FromDate FromDate
            ,@ToDate ToDate
            ,0 AS DeductDays
            ,CASE WHEN e.LastYearAL > 3 THEN 3 ELSE e.LastYearAL END LastYearMaxAL
            ,dbo.fnc_GetClientDateTime()
            ,NULL CreatedBy
            ,0 Deleted
            ,0 as IsOpenBalance
        FROM #EmployeeLastYearBalance e
		WHERE e.LastYearAL > 0 AND e.IsMaxAL = 0
        COMMIT TRANSACTION;

		DROP TABLE #EmployeeLastYearBalance;

        SELECT @ErrCode = 0
            ,@ErrCode = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION;

        SELECT @ErrCode = 100
            ,@ErrMsg = 'Transfer AL from ' + CONVERT(NVARCHAR(10), @Year) + ' failed'

        RETURN
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[DBA_AnnualLeave_TransferBalanceFromYear_xtest] @Year INT
    ,@EmpID INT = NULL
    ,@Description NVARCHAR(1000) = ''
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Oliver@2024.12.31
Task: Transfer AL unused from last year to current year
DB: NexHRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF LEN(ISNULL(@Description, '')) = 0
        SET @Description = 'Transfer AL from ' + CONVERT(NVARCHAR(10), @Year) + ''

    DECLARE @FromDate DATETIME
        ,@ToDate DATETIME

    SELECT @FromDate = CONVERT(NVARCHAR(10), @Year + 1) + '/01/01'
        ,@ToDate = CONVERT(NVARCHAR(10), @Year + 1) + '/03/31'

    BEGIN TRY
        BEGIN TRANSACTION;
		
		INSERT INTO dbo.LateInEarlyOutYearly(EmpId, Minutes, Year, ModifiedBy, ModifiedDate)
		SELECT e.Id
		,(
			ISNULL(la.[Minutes], 0) - CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
			,@Year + 1
			,NULL
			,dbo.fnc_GetClientDateTime()
		FROM dbo.Employee AS e WITH (NOLOCK)
			LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
			AND la.[Year] = @Year
			LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
			AND oty.[Year] = @Year
		WHERE e.IsSpecialUser = 0
			AND ISNULL(e.isDeleted, 0) = 0

		CREATE TABLE #LastYearLIEO (EmpId INT, [Minutes] INT);
		INSERT INTO #LastYearLIEO (EmpId, [Minutes])
		SELECT e.Id
			,(
				ISNULL(la.[Minutes], 0) - CASE 
				WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
				ELSE ISNULL(la.[Minutes], 0) END
				)
		FROM dbo.Employee AS e WITH (NOLOCK)
			LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
			AND la.[Year] = @Year
			LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
			AND oty.[Year] = @Year
		WHERE e.IsSpecialUser = 0
			AND ISNULL(e.isDeleted, 0) = 0

		INSERT INTO dbo.LateInEarlyOutYearly(EmpId, Year, Minutes, ModifiedBy, ModifiedDate)
		SELECT li.EmpId, @Year + 1, li.Minutes, NULL, dbo.fnc_GetClientDateTime()
		FROM #LastYearLIEO li
		WHERE NOT EXISTS (SELECT 1 FROM dbo.LateInEarlyOutYearly WHERE EmpId = li.EmpId AND Year = @Year + 1)

		CREATE TABLE #EmployeeLastYearBalance
		 (EmpId          INT,
		  PreYearAL NUMERIC(18,1),
		  IsMaxAL BIT,
		  PreYearLIEO INT
		 );

		INSERT INTO #EmployeeLastYearBalance(EmpId, PreYearAL, IsMaxAL, PreYearLIEO)
		SELECT e.Id
				,SUM(lt.CreditDays) - SUM(lt.DeductDays) as LastYearAL
				,(CASE 
				WHEN EXISTS (SELECT 1 FROM dbo.LeaveTrans with (nolock) where EmpId = e.Id and CreditDays >= 29 and YEAR(FromDate) = @Year + 1) 
				THEN 1 ELSE 0
			END) as IsMaxAL
				,pLi.Minutes
		FROM [dbo].[LeaveTrans] lt with (nolock)
		INNER JOIN dbo.Employee e with (nolock) on lt.EmpId = e.Id
		LEFT JOIN #LastYearLIEO pLI on e.Id = pLi.EmpId
		WHERE YEAR(lt.FromDate) = 2024 AND ISNULL(lt.Deleted, 0) = 0
			AND ISNULL(isdeleted, 0) = 0
			AND e.[Enabled] = 1
			AND (
				e.Id = @EmpID
				OR @EmpID IS NULL
				)
		GROUP BY lt.EmpId, e.Id, e.NickName, pLI.Minutes;

		UPDATE ly 
		SET ly.Minutes = 0
		FROM dbo.LateInEarlyOutYearly ly
		JOIN #EmployeeLastYearBalance tmp ON ly.EmpId = tmp.EmpId
		WHERE (tmp.IsMaxAL = 1 AND PreYearAL > 0) OR (tmp.IsMaxAL = 0 AND PreYearAL > 3) AND tmp.PreYearLIEO > 0
		
		INSERT INTO dbo.LeaveTrans (
            EmpId
            ,[Description]
            ,FromDate
            ,ToDate
            ,DeductDays
            ,CreditDays
            ,CreatedDate
            ,CreatedBy
            ,Deleted
            ,IsOpenBalance
            )
        SELECT e.EmpId
            ,'Transfer AL to ' + CONVERT(NVARCHAR(10), @Year + 1) + ''
            ,CONVERT(NVARCHAR(10), @Year) + '/12/31'
            ,NULL
            ,CASE WHEN e.PreYearAL > 3 THEN 3 ELSE e.PreYearAL END LastYearMaxAL
            ,0
            ,dbo.fnc_GetClientDateTime()
            ,NULL CreatedBy
            ,0 Deleted
            ,0 as IsOpenBalance
        FROM #EmployeeLastYearBalance e
		WHERE e.PreYearAL > 0 AND e.IsMaxAL = 0
		
        INSERT INTO dbo.LeaveTrans (
            EmpId
            ,[Description]
            ,FromDate
            ,ToDate
            ,DeductDays
            ,CreditDays
            ,CreatedDate
            ,CreatedBy
            ,Deleted
            ,IsOpenBalance
            )
        SELECT e.EmpId
            ,@Description Description
            ,@FromDate FromDate
            ,@ToDate ToDate
            ,0 AS DeductDays
            ,CASE WHEN e.PreYearAL > 3 THEN 3 ELSE e.PreYearAL END LastYearMaxAL
            ,dbo.fnc_GetClientDateTime()
            ,NULL CreatedBy
            ,0 Deleted
            ,0 as IsOpenBalance
        FROM #EmployeeLastYearBalance e
		WHERE e.PreYearAL > 0 AND e.IsMaxAL = 0

        COMMIT TRANSACTION;

		DROP TABLE #LastYearLIEO;
		DROP TABLE #EmployeeLastYearBalance;

        SELECT @ErrCode = 0
            ,@ErrCode = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION;

        SELECT @ErrCode = 100
            ,@ErrMsg = 'Transfer AL from ' + CONVERT(NVARCHAR(10), @Year) + ' failed'

        RETURN
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[DBA_ReIndex]
AS
    -- Joe : reorganize all index with fragment > 30%
    -- Dan: reorganize all index with fragment > 10 %, rebuild for fragment > 30 %

    declare @database_id int
    select @database_id = db_id()

    declare @CMDs TABLE (rn int PRIMARY KEY IDENTITY(1,1), cmd nvarchar(1000))			

    INSERT INTO @CMDs (cmd)
    SELECT
    'ALTER INDEX [' + i.name + '] ON [dbo].[' + tbl.name + '] ' + 
	    CASE WHEN fi.avg_fragmentation_in_percent > 30 then 'REBUILD'
	    ELSE 'REORGANIZE'
	    END
    --fi.avg_fragmentation_in_percent AS [AverageFragmentation]
    FROM
    sys.tables AS tbl
    INNER JOIN sys.indexes AS i ON (i.index_id > 0 and i.is_hypothetical = 0) AND (i.object_id=tbl.object_id)
    INNER JOIN sys.dm_db_index_physical_stats(@database_id, NULL, NULL, NULL, 'LIMITED') AS fi ON fi.object_id=CAST(i.object_id AS int) AND fi.index_id=CAST(i.index_id AS int)
    WHERE
	    fi.avg_fragmentation_in_percent > 10
    --	AND tbl.name NOT LIKE 'BalanceRecord%'
    ORDER BY tbl.name, i.type_desc

    declare @rn int, @cmd nvarchar(1000)
    SET @rn = 1

    WHILE EXISTS(SELECT 1 FROM @CMDs WHERE rn = @rn)
    BEGIN
	    SELECT @cmd = cmd FROM @CMDs WHERE rn = @rn
	    SET @rn = @rn + 1

	    EXEC sp_executesql @cmd
    END

    EXEC sp_updatestats
    --DBCC DROPCLEANBUFFERS
GO

CREATE OR ALTER PROCEDURE [dbo].[Department_Delete] 
    @DepartmentId    INT
    ,@ModifiedBy INT
    ,@ModifiedDate DATETIME
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140610@Quinn.Vo
Task: delete Department.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @DepartmentLevel INT

    IF (
            NOT EXISTS (
                SELECT 1
                FROM dbo.Department WITH (NOLOCK)
                WHERE DepartmentId = @DepartmentId
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The department does not exists '

        RETURN
    END

    IF (
        EXISTS (
            SELECT 1
            FROM dbo.Department WITH (NOLOCK)
            WHERE [ParentId] = @DepartmentId
        )
    )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The department has child departments '

        RETURN
    END
    
    IF (
        EXISTS (
            SELECT 1
            FROM dbo.Department AS dep WITH (NOLOCK)
            INNER JOIN dbo.EmployeeGroup AS gr WITH (NOLOCK) ON dep.DepartmentId = gr.DepartmentId
            WHERE dep.DepartmentId = @DepartmentId AND gr.Deleted != 1
        )
    )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The department has teams '

        RETURN
    END
    
    DELETE FROM dbo.Department
    WHERE DepartmentId = @DepartmentId

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Department_Get]
	@ParentId INT = NULL
	,@IsDisabled BIT = NULL
	,@IsSpecial BIT = NULL
AS
/*
Created: 20140818@Josh.Phan
Task: Get departments by departmentId.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT dep.[DepartmentId] AS Id
		,dep.[Code]
		,dep.[Name]
		,dep.[ParentId]
		,dep2.[Name] AS ParentName
		,dep.[ModifiedDate]
		,dep.[CreatedDate]
		,dep.[IsDisabled]
		,dep.[DepartmentLevel]
		,dep.[DisplayOrder]
		,dep.[CreatedBy]
		,cr.FullName
		,cr.NickName
		,dep.[ManagerId]
		,mg.FullName
		,mg.NickName
		,dep.[ModifiedBy]
		,md.FullName
		,md.NickName
	FROM dbo.Department AS dep WITH (NOLOCK)
	LEFT JOIN dbo.Employee AS cr WITH (NOLOCK) ON dep.CreatedBy = cr.Id
	LEFT JOIN dbo.Employee AS mg WITH (NOLOCK) ON dep.[ManagerId] = mg.Id
	LEFT JOIN dbo.Employee AS md WITH (NOLOCK) ON dep.[ModifiedBy] = md.Id
	LEFT JOIN dbo.Department AS dep2 WITH (NOLOCK) ON dep.ParentId = dep2.DepartmentId
	WHERE (
			@ParentId IS NULL
			OR dep.ParentId = @ParentId
			)
		AND (
			@IsDisabled IS NULL
			OR dep.IsDisabled = @IsDisabled
			)
		AND (
			@IsSpecial IS NULL
			OR dep.IsSpecial = @IsSpecial
			)
    ORDER BY dep.DisplayOrder, dep.Name
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Department_GetById]
    @DepartmentId INT
AS
/*
Created: 20140819@Josh.Phan
Task: Get by departmentId.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT dep.[DepartmentId]
        ,dep.[Code]
        ,dep.[Name]
        ,dep.[ParentId]
        ,dep2.[Name] AS ParentName
        ,dep.[ModifiedDate]
        ,dep.[CreatedDate]
        ,dep.[IsDisabled]
        ,dep.[DepartmentLevel]
        ,dep.[DisplayOrder]
        ,dep.[CreatedBy]
        ,cr.FullName
        ,cr.NickName
        ,dep.[ManagerId]
        ,mg.FullName
        ,mg.NickName
        ,dep.[ModifiedBy]
        ,md.FullName
        ,md.NickName
    FROM dbo.Department AS dep WITH (NOLOCK)
    LEFT JOIN dbo.Employee AS cr WITH (NOLOCK) ON dep.CreatedBy = cr.Id
    LEFT JOIN dbo.Employee AS mg WITH (NOLOCK) ON dep.[ManagerId] = mg.Id
    LEFT JOIN dbo.Employee AS md WITH (NOLOCK) ON dep.[ModifiedBy] = md.Id
    LEFT JOIN dbo.Department AS dep2 WITH (NOLOCK) ON dep.ParentId = dep2.DepartmentId
    WHERE @DepartmentId = dep.DepartmentId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Department_Insert] 
    @Code VARCHAR(20)
    ,@Name VARCHAR(100)
    ,@ParentId INT = NULL
    ,@ManagerId INT = NULL
    ,@CreatedBy INT
    ,@CreatedDate DATETIME
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140604@Quinn.Vo
Task: Insert Department.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @DepartmentLevel INT

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Department WITH (NOLOCK)
                WHERE Code = @Code
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Department code' + @Code + ' has already exists'

        RETURN
    END

    SELECT @DepartmentLevel = DepartmentLevel 
    FROM dbo.Department WITH (NOLOCK)
    WHERE DepartmentId = @ParentId

    IF(@DepartmentLevel IS NULL)
    BEGIN
        SET @DepartmentLevel = 0
    END
    ELSE
    BEGIN
        SET @DepartmentLevel = @DepartmentLevel + 1
    END

    INSERT INTO dbo.Department(
        [Code]
        ,[Name]
        ,[ParentId]
        ,[ManagerId]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[IsDisabled]
        ,DepartmentLevel
        )
    VALUES (
        @Code
        ,@Name
        ,@ParentId
        ,@ManagerId
        ,@CreatedBy
        ,@CreatedDate
        ,0
        ,@DepartmentLevel
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Department_Update] 
    @DepartmentId    INT
    ,@Code VARCHAR(20)
    ,@Name VARCHAR(100)
    ,@ParentId INT = NULL
    ,@ManagerId INT = NULL
    ,@IsDisabled BIT = NULL
    ,@ModifiedBy INT
    ,@ModifiedDate DATETIME
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140604@Quinn.Vo
Task: Update Department.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @DepartmentLevel INT,
        @OldDepartmentLevel INT,
        @SubLevel INT

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Department WITH (NOLOCK)
                WHERE Code = @Code
                AND DepartmentId <> @DepartmentId
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Department code' + @Code + ' has already exists'

        RETURN
    END

    SELECT @DepartmentLevel = DepartmentLevel 
    FROM dbo.Department WITH (NOLOCK)
    WHERE DepartmentId = @ParentId

    SELECT @OldDepartmentLevel = DepartmentLevel 
    FROM dbo.Department WITH (NOLOCK)
    WHERE DepartmentId = @DepartmentId

    IF(@DepartmentLevel IS NULL)
    BEGIN
        SET @DepartmentLevel = 0
    END
    ELSE
    BEGIN
        SET @DepartmentLevel = @DepartmentLevel + 1
    END

    UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
    SET 
        [Code] = @Code,
        [Name] = @Name,
        [ParentId] = @ParentId,
        [ManagerId] = @ManagerId,
        [DepartmentLevel] = @DepartmentLevel,
        [IsDisabled] = @IsDisabled,
        [ModifiedBy] = @ModifiedBy,
        [ModifiedDate] = @ModifiedDate
    WHERE DepartmentId = @DepartmentId

    SET @SubLevel = @DepartmentLevel - @OldDepartmentLevel
    
    UPDATE dp 
    SET [DepartmentLevel] = (d.DepartmentLevel + @SubLevel)
    FROM dbo.Department AS dp WITH (ROWLOCK, UPDLOCK)
    INNER JOIN dbo.RelatedDepartment(@DepartmentId) AS d ON dp.DepartmentId = d.DepartmentId

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update department failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_Delete] @employeeId INT
    ,@ResignedDate SMALLDATETIME
	,@ModifiedBy INT = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: Delete employee
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    IF (@ResignedDate IS NULL)
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'The user cannot be deleted without resigned date'

        RETURN
    END

    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.EmployeeExt WITH (UPDLOCK, ROWLOCK)
        SET ResignedDate = @ResignedDate
            ,isDeleted = 1
        WHERE ID = @employeeId

		UPDATE HRM.dbo.Employee WITH (UPDLOCK, ROWLOCK)
        SET ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE ID = @employeeId

        DELETE dbo.EmployeeCalendar
        WHERE WorkDate > @ResignedDate
            AND empid = @employeeId;

        DELETE dbo.EmployeeAttendance
        WHERE AttendanceDate > @ResignedDate
            AND empid = @employeeId

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Delete Employee unsuccessfully.'
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_Get] @FromDate DATETIME = NULL
AS
/*
Created: 20140320@Quinn.Vo
Task: Get all employee from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT e.Id AS EmpId
        ,'' AS [Password]
        ,e.Code
        ,e.NickName
        ,e.FullName
        ,e.EmployeeCard
        ,e.DomainUsername AS DomainName
        ,e.[Enabled] AS IsEnabled
        ,ISNULL(e.isDeleted, 0) AS IsDeleted
        ,e.JoinedDate
        ,e.GroupId AS TeamId
        ,CASE 
            WHEN e.ModifiedDate IS NULL THEN e.CreatedDate
            ELSE e.ModifiedDate
            END AS ModifiedDate
    FROM dbo.Employee AS e WITH (NOLOCK)
    WHERE (@FromDate IS NULL
        OR e.ModifiedDate > @FromDate)
		AND e.IsSpecialUser = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetBirthdayAndFirstWorkingday] @OnDate DATETIME
AS
/*
Created: 20160218@Quinn.Vo
Task: get employee has birthday or first working day on date.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SELECT e.Id
		,e.Code
		,e.DomainUsername
		,e.FullName
		,e.NickName
		,e.DOB
		,e.DOB AS DateOfBirth
		,e.Email
		,e.JobTitleId
		,e.Enabled
		,e.Gender
		,e.GroupId
		,e.Roles
		,e.GroupDate AS JoinedDate
		,e.ConfirmedDate
		,e.Skype
		,e.Yahoo
		,e.Marriage
		,e.CreatedDate
		,e.CreatedBy
		,e.ModifiedDate
		,e.ModifiedBy
		,e.isDeleted
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
	WHERE (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND MONTH(DOB) = MONTH(@OnDate)
		AND DAY(DOB) = DAY(@OnDate)
		AND (
			ResignedDate IS NULL
			OR DATEDIFF(DD, dbo.fnc_GetClientDateTime(), ResignedDate) >= 0
			)
		AND e.Enabled = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0

	SELECT e.Id
		,e.Code
		,e.DomainUsername
		,e.FullName
		,e.NickName
		,e.DOB AS DateOfBirth
		,e.Email
		,e.JobTitleId
		,e.Enabled
		,e.Gender
		,e.GroupId
		,e.Roles
		,e.GroupDate AS JoinedDate
		,e.ConfirmedDate
		,e.Skype
		,e.Yahoo
		,e.Marriage
		,e.CreatedDate
		,e.CreatedBy
		,e.ModifiedDate
		,e.ModifiedBy
		,e.isDeleted
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
	WHERE (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND MONTH(GroupDate) = MONTH(@OnDate)
		AND DAY(GroupDate) = DAY(@OnDate)
		AND GroupDate < convert(VARCHAR(10), dbo.fnc_GetClientDateTime(), 111)
		AND (
			ResignedDate IS NULL
			OR DATEDIFF(DD, dbo.fnc_GetClientDateTime(), ResignedDate) >= 0
			)
		AND e.Enabled = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
		
		EXEC [CompanyEvent_GetEventInWeek]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetByDomainName] @DomainUsername VARCHAR(50)
AS
/*
Created: 20160205@Quinn.Vo
Task: Get employee info by domain username.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT e.[Id]
	,e.[Code]
	,e.[FullName]
	,e.[NickName]
	,e.[Email]
	,e.[Enabled]
	,e.[IsDeleted]
	,e.[Roles]
	,ISNULL(g.Id, 0) AS TeamId
	,g.Code AS TeamCode
	,CAST(CASE WHEN e.Id = 125 OR e.Id = 142 THEN 1 ELSE 0 END AS BIT) AS IsLeaderHR
	FROM dbo.Employee AS e WITH (NOLOCK) 
		LEFT JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
	WHERE e.DomainUsername = @DomainUsername
	AND e.isDeleted = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetById] @EmployeeId INT
AS
/*
Created: 20160205@Quinn.Vo
Task: Get employee info by id
DB: NexHRM
Original: 

Revisions:  
	20191206@Merlin.Le: Add PlaceOfBirth, ContactName, UserRelationship, ContactPhone, ContactAddress to table Employee
	20252602@Henry.Dinh: Add Management Grade
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATETIME
        ,@AnnualLeaveBalancePending NUMERIC(5, 1)
        ,@AnnualLeaveBalance NUMERIC(5, 1)
        ,@SubGroup VARCHAR(50)
		,@Approvers VARCHAR(255)

    SET @Today = dbo.fnc_GetClientDateTime()

    SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
    FROM dbo.LeaveTrans WITH (NOLOCK)
    WHERE EmpId = @EmployeeId
        AND ISNULL(Deleted, 0) = 0
        AND YEAR(FromDate) = YEAR(@Today)

    SELECT @AnnualLeaveBalancePending = SUM(ISNULL(wk.TotalDays, 0))
    FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON e.Id = wk.EmpId
    WHERE e.Id = @EmployeeId
        AND wk.RequestType = 6
        AND wk.[Status] = 1
        AND wk.LeaveTypeID IN (2, 30)
    GROUP BY e.Id

    SELECT @SubGroup = ''

    SELECT @SubGroup = @SubGroup + COALESCE(CAST(eg.GroupId AS VARCHAR(10)) + ',', '')
    FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
    WHERE eg.EmpID = @EmployeeId
        AND ISNULL(isMainGroup, 0) = 0
		AND ISNULL(HandleRequest, 0) = 0

    SELECT @SubGroup = CASE 
            WHEN LEN(@SubGroup) > 0 THEN LEFT(@SubGroup, LEN(@SubGroup) - 1)
            ELSE ''
            END

    SELECT e.[Id]
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
		,[EducationBackground]
        ,[DOB]
        ,[Email]
        ,[Enabled]
        ,[Phone]
        ,[Gender]
        ,[Roles]
        ,[JoinedDate]
        ,[ConfirmedDate]
        ,[ResignedDate]
        ,[Skype]
        ,[Yahoo]
        ,[Marriage]
        ,ISNULL(@AnnualLeaveBalance, 0) - ISNULL(@AnnualLeaveBalancePending, 0) AS AnnualLeaveBalance
        ,@SubGroup AS SubGroups
		,gra.JobGradeId
		,gra.Code AS JobGradeCode
		,gra.Name AS JobGradeName
		,gra.Description
		,gra.AffectedDate
		,gra.Note
        ,mgra.ManagementGradeId
		,mgra.Code AS ManagementGradeCode
		,mgra.Name AS ManagementGradeName
		,mgra.Description AS ManagementDescription
		,mgra.AffectedDate AS ManagementAffectedDate
		,mgra.Note AS ManagementNote
        ,e.SalaryGroupId
		,e.SocialInsuranceNo
        ,e.[CreatedDate]
        ,e.[CreatedBy]
        ,e.[ModifiedDate]
        ,e.[ModifiedBy]
        ,e.[IsDeleted]
		,[IsSpecialUser]
		,[GroupDate]
        ,[PermanentAddress] AS [Address]
        ,[CurrentAddress]
        ,[IdCardNumber]
        ,[PassportNo]
        ,[PassportExp]
        ,[GeoMapLatitude]
        ,[GeoMapLongtitude]
        ,[DistanceToCompany]
        ,[EmployeeCard]
        ,[IssuedDate]
        ,[IssuedPlace]
		,[PlaceOfBirth]
		,[ContactName]
		,[UserRelationship]
		,[ContactPhone]
		,[ContactAddress]
        ,ISNULL([MonShift], 0) AS MonShiftId
        ,ISNULL(sh2.NAME, 'Off') AS MonShiftName
        ,ISNULL([TueShift], 0) AS TueShiftId
        ,ISNULL(sh3.NAME, 'Off') AS TueShiftName
        ,ISNULL([WedShift], 0) AS WedShiftId
        ,ISNULL(sh4.NAME, 'Off') AS WedShiftName
        ,ISNULL([ThuShift], 0) AS ThuShiftId
        ,ISNULL(sh5.NAME, 'Off') AS ThuShiftName
        ,ISNULL([FriShift], 0) AS FriShiftId
        ,ISNULL(sh6.NAME, 'Off') AS FriShiftName
        ,ISNULL([SatShift], 0) AS SatShiftId
        ,ISNULL(sh7.NAME, 'Off') AS SatShiftName
        ,ISNULL([SunShift], 0) AS SunShiftId
        ,ISNULL(sh8.NAME, 'Off') AS SunShiftName
        ,LRExamDate
        ,Listening
        ,OldListening
        ,Reading
        ,OldReading
        ,SWExamDate
        ,Speaking
        ,OldSpeaking
        ,Writing
        ,OldWriting
        ,InputDate
        ,ISNULL(GroupId, 0) AS GroupId
        ,g.Code
        ,g.NAME
        ,ISNULL(e.JobTitleId, 0) AS JobTitleId
        ,jt.[Name]
    FROM dbo.Employee AS e WITH (NOLOCK)
        LEFT JOIN dbo.EmployeeShift AS es WITH (NOLOCK) ON e.Id = es.EmpID
        LEFT JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
        LEFT JOIN HRM.dbo.TOEIC AS toeic WITH (NOLOCK) ON e.Id = toeic.EmpId
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
        LEFT JOIN dbo.[Shift] AS sh2 WITH (NOLOCK) ON es.MonShift = sh2.Id
        LEFT JOIN dbo.[Shift] AS sh3 WITH (NOLOCK) ON es.TueShift = sh3.Id
        LEFT JOIN dbo.[Shift] AS sh4 WITH (NOLOCK) ON es.WedShift = sh4.Id
        LEFT JOIN dbo.[Shift] AS sh5 WITH (NOLOCK) ON es.ThuShift = sh5.Id
        LEFT JOIN dbo.[Shift] AS sh6 WITH (NOLOCK) ON es.FriShift = sh6.Id
        LEFT JOIN dbo.[Shift] AS sh7 WITH (NOLOCK) ON es.SatShift = sh7.Id
        LEFT JOIN dbo.[Shift] AS sh8 WITH (NOLOCK) ON es.SunShift = sh8.Id
		LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.JobGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeJobGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.JobGrade as jg WITH (NOLOCK) on ejg.JobGradeId = jg.JobGradeId
			  WHERE ejg.EmpId = @EmployeeId 
			  AND ejg.IsCurrentUsed = 1
			  AND ISNULL(ejg.IsDeleted,0) = 0
		) AS gra ON e.Id = gra.EmpId
		LEFT JOIN (
			SELECT emg.EmpId
			,emg.AffectedDate
			,emg.Note
			,mg.ManagementGradeId
			,mg.Code
			,mg.Name
			,mg.Description
			  FROM HRM.dbo.EmployeeManagementGrade as emg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.ManagementGrade as mg WITH (NOLOCK) on emg.ManagementGradeId = mg.ManagementGradeId
			  WHERE emg.EmpId = @EmployeeId 
			  AND emg.IsCurrentUsed = 1
			  AND ISNULL(emg.IsDeleted,0) = 0
		) AS mgra ON e.Id = mgra.EmpId

    WHERE e.Id = @EmployeeId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetByModifiedDate] @ModifiedDate DateTime
AS
/*
Created: 20230205@Quinn.Vo
Task: Get employee info by username.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT e.[Id]
	,e.[Code]
	,e.DomainUsername
	,e.[FullName]
	,e.[NickName] AS EnglishName
	,e.DOB AS DateOfBirth
	,e.PlaceOfBirth
	,e.[Email]
	,e.EmployeeCard
	,g.Code AS TeamCode
	,g.Name AS TeamName
	,jt.Name AS JobTitle
	,gra.Code AS JobGrade
	,e.GroupDate
	,e.JoinedDate
	,e.ConfirmedDate
	,e.ResignedDate
	,e.Phone
	,(CASE WHEN e.Gender = 0 THEN 'F'
		WHEN e.Gender = 1 THEN 'M'
		ELSE 'Unknown' END) AS Gender
	,e.IdCardNumber
	,e.IssuedDate
	,e.IssuedPlace
	,e.Phone
	,e.CurrentAddress
	,e.PermanentAddress
	,e.PassportNo
	,e.PassportExp
	,e.SocialInsuranceNo
	,e.Skype
	,e.Yahoo AS Telegram
	,e.[Enabled]
	,e.[IsDeleted]
	,e.Marriage
	,e.CreatedDate
	,e.ModifiedDate
	,e.Enabled
	,e.isDeleted
	,e.IsSpecialUser
	,e.ContactName AS EmergencyContact
	,e.UserRelationship AS EmergencyRelationship
	,e.ContactPhone AS EmergencyPhone
	,e.ContactAddress AS EmergencyAddress
	,t.LRExamDate AS ToeicExamDate
    ,t.Listening AS ToeicListening
	,t.Reading AS ToeicReading
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.NickName ELSE egs.NickName END AS SupervisorEnglishName
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.FullName ELSE egs.FullName END AS SupervisorFullName
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.Email ELSE egs.Email END AS SupervisorEmail
	FROM dbo.Employee AS e WITH (NOLOCK) 
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
		LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON gs.GroupId = e.GroupId
		LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON es.EmpId = e.Id
		LEFT JOIN dbo.Employee AS egs WITH (NOLOCK) ON gs.ApproverId = egs.Id
		LEFT JOIN dbo.Employee AS egs2 WITH (NOLOCK) ON es.ApproverId = egs2.Id
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		LEFT JOIN HRM.dbo.TOEIC AS t WITH (NOLOCK) ON e.Id = t.EmpId
		LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.JobGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeJobGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.JobGrade as jg WITH (NOLOCK) on ejg.JobGradeId = jg.JobGradeId
			  WHERE ejg.IsCurrentUsed = 1
			  AND ISNULL(ejg.IsDeleted,0) = 0
		) AS gra ON e.Id = gra.EmpId
	WHERE e.ModifiedDate BETWEEN @ModifiedDate AND dbo.fnc_GetClientDateTime()
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetByNickName] @NickName VARCHAR(50)
AS
/*
Created: 20180829@Quinn.Vo
Task: Get employee info by nick name.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @EmployeeId INT

    SELECT @EmployeeId = Id
    FROM dbo.Employee WITH (NOLOCK)
    WHERE NickName = @NickName

    EXECUTE dbo.Employee_GetById @EmployeeId = @EmployeeId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetByUsername] @Username VARCHAR(50)
AS
/*
Created: 20230205@Quinn.Vo
Task: Get employee info by username.
DB: NexHRM
Original: 

Revisions:  
    20250520@Henry.Dinh: Add DepartmentName.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	DECLARE @DomainName VARCHAR(20)

	SELECT @DomainName = Value
	FROM dbo.SystemSetting WITH (NOLOCK)
	WHERE [Name] = 'DomainName'

	SELECT e.[Id]
	,e.[Code]
	,e.DomainUsername
	,e.[FullName]
	,e.[NickName] AS EnglishName
	,e.DOB AS DateOfBirth
	,e.PlaceOfBirth
	,e.[Email]
	,e.EmployeeCard
    ,d.Name AS DepartmentName
	,g.Code AS TeamCode
	,g.Name AS TeamName
	,jt.Name AS JobTitle
	,gra.Code AS JobGrade
	,e.GroupDate
	,e.JoinedDate
	,e.ConfirmedDate
	,e.ResignedDate
	,e.Phone
	,(CASE WHEN e.Gender = 0 THEN 'F'
		WHEN e.Gender = 1 THEN 'M'
		ELSE 'Unknown' END) AS Gender
	,e.IdCardNumber
	,e.IssuedDate
	,e.IssuedPlace
	,e.Phone
	,e.CurrentAddress
	,e.PermanentAddress
	,e.PassportNo
	,e.PassportExp
	,e.SocialInsuranceNo
	,e.Skype
	,e.Yahoo AS Telegram
	,e.[Enabled]
	,e.[IsDeleted]
	,e.Marriage
	,e.CreatedDate
	,e.ModifiedDate
	,e.Enabled
	,e.isDeleted
	,e.IsSpecialUser
	,e.ContactName AS EmergencyContact
	,e.UserRelationship AS EmergencyRelationship
	,e.ContactPhone AS EmergencyPhone
	,e.ContactAddress AS EmergencyAddress
	,t.LRExamDate AS ToeicExamDate
    ,t.Listening AS ToeicListening
	,t.Reading AS ToeicReading
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.NickName ELSE egs.NickName END AS SupervisorEnglishName
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.FullName ELSE egs.FullName END AS SupervisorFullName
	,CASE WHEN egs2.Id IS NOT NULL THEN egs2.Email ELSE egs.Email END AS SupervisorEmail
	FROM dbo.Employee AS e WITH (NOLOCK) 
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN dbo.Department AS d WITH (NOLOCK) ON g.DepartmentId = d.DepartmentId
		LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON gs.GroupId = e.GroupId
		LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON es.EmpId = e.Id
		LEFT JOIN dbo.Employee AS egs WITH (NOLOCK) ON gs.ApproverId = egs.Id
		LEFT JOIN dbo.Employee AS egs2 WITH (NOLOCK) ON es.ApproverId = egs2.Id
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		LEFT JOIN HRM.dbo.TOEIC AS t WITH (NOLOCK) ON e.Id = t.EmpId
		LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.JobGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeJobGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.JobGrade as jg WITH (NOLOCK) on ejg.JobGradeId = jg.JobGradeId
			  WHERE ejg.IsCurrentUsed = 1
			  AND ISNULL(ejg.IsDeleted,0) = 0
		) AS gra ON e.Id = gra.EmpId
	WHERE e.DomainUsername = (@DomainName + @Username)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetEmployeeByTeams] @employeeId INT
    ,@teamCodes VARCHAR(200) = ''
    ,@ignoreTeams VARCHAR(200) = ''
AS
/*  
Created: 20120810@Quinn.Vo
Task: Send mail for all Staff in PM team
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #TeamCodeList (Code VARCHAR(20));

    CREATE TABLE #IgnoreTeamList (Code VARCHAR(20));

    INSERT INTO #TeamCodeList (Code)
    SELECT val
    FROM fnc_strSplit(@teamCodes, ',');

    INSERT INTO #IgnoreTeamList (Code)
    SELECT val
    FROM fnc_strSplit(@ignoreTeams, ',');

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Employee AS e WITH (NOLOCK)
                    INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
                WHERE e.Id = @employeeId
                    AND g.Code NOT IN (
                        SELECT Code
                        FROM #IgnoreTeamList
                        )
                )
            )
    BEGIN
        SELECT e.Id
            ,e.Code
            ,e.NickName
            ,e.FullName
            ,e.Email
            ,e.DomainUsername
        FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
            INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON eg.GroupId = g.Id
            INNER JOIN dbo.Employee AS e ON e.Id = eg.EmpID
        WHERE g.Code IN (
                SELECT Code
                FROM #TeamCodeList
                )
            AND (
                e.isDeleted IS NULL
                OR e.isDeleted = 0
                )
	       AND e.[Enabled] = 1
		   AND (ISNULL(eg.HandleRequest, 0) = 0 OR isMainGroup = 1)
    END;
    ELSE
    BEGIN
        SELECT e.Id
            ,e.Code
            ,e.NickName
            ,e.FullName
            ,e.Email
            ,e.DomainUsername
        FROM dbo.Employee AS e WITH (NOLOCK)
        WHERE 1 = 0;
    END;

    DROP TABLE #TeamCodeList;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetEmployeeForRelatedTeam] @empId INT
AS

/*  
Created: 2012/08/10@Quinn  
Task: Send mail for all Staff with related Team  
DB: NexHRM  
Original:   
  
Revisions:      
Param's Explanation:  
  
*/

BEGIN
    SET NOCOUNT ON;

    SELECT e.Id,
        e.Code,
        e.NickName,
        e.FullName,
        e.Email,
        e.DomainUsername
    FROM dbo.Employee AS e WITH (NOLOCK)
    WHERE E.ID IN
    (
        SELECT gg.EmpID
        FROM dbo.EmployeeGroup_Employee AS gg WITH (NOLOCK)
        WHERE gg.GroupId IN
        (
            SELECT eg.GroupId
            FROM dbo.EmployeeGroup_Employee AS eg WITH (NOLOCK)
            WHERE eg.EmpID = @empId
        )
    )
        AND ISNULL(e.isDeleted, 0) = 0;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetEmployees] @query NVARCHAR(256)
    ,@groupId INT = NULL
    ,@departmentId INT = NULL
    ,@jobTitleId INT = NULL
	,@IsDisable BIT = 0
    ,@isDelete BIT = 0
    ,@pageNumber INT = 1
    ,@pageSize INT = 2000
    ,@isSpecial BIT = NULL
    ,@totalRow INT OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: Get employee by conditions.
DB: NexHRM
Original: 

Revisions:  
	20191206@Merlin.Le: Add PlaceOfBirth, ContactName, UserRelationship, ContactPhone, ContactAddress to table Employee
	20200505@Merlin.Le: Add isSpecial IN PARAM
	20250308@Henry.Dinh: Add management grade
    20250514@Henry.Dinh: Add departmentId IN PARAM
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @q NVARCHAR(256)

    SET @q = '%' + @query + '%'

    IF LEN(@query) = 0
        SET @query = NULL

    SELECT @totalRow = COUNT(1)
    FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.ID
    WHERE (
            @isDelete IS NULL
            OR isDeleted = @isDelete
            )
		AND (@isDelete = 1
						AND ISNULL(isDeleted, 0) = 1 
			OR (

						@isDelete = 0
						AND ISNULL(isDeleted, 0) = 0 
						AND
						(
							[Enabled] <> @isDisable
						)
					
				)
				)
        AND (
            @query IS NULL
            OR NickName LIKE @q
            OR FullName LIKE @q
            OR DomainUsername LIKE @q
            OR e.Code LIKE @q
            )
        AND (
            @groupId IS NULL
            OR GroupId = @groupId
            )
        AND (
            @departmentId IS NULL
            OR DepartmentId = @departmentId
            )

    SELECT e.[Id]
		,e.[Id] as UserId
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
		,[GroupDate]
        ,[DOB]
        ,[Email]
        ,[Enabled]
        ,[Phone]
        ,[Gender]
        ,[Roles]
        ,[JoinedDate]
        ,[ConfirmedDate]
        ,[ResignedDate]
        ,[Skype]
        ,[Yahoo]
        ,[Marriage]
        ,d.DepartmentId
		,d.[Name] AS DepartmentName
		,gra.Code AS JobGradeCode
		,gra.Name AS JobGradeName
		,gra.AffectedDate
		,gra.Note
		,mgra.Code AS ManagementGradeCode
		,mgra.Name AS ManagementGradeName
		,mgra.AffectedDate AS ManagementAffectedDate
		,mgra.Note AS ManagementNote
		,e.SocialInsuranceNo
        ,e.[CreatedDate]
        ,e.[CreatedBy] CreatorId
        ,e.[ModifiedDate]
        ,e.[ModifiedBy]
        ,e.[isDeleted]
        ,[PermanentAddress]
        ,[CurrentAddress]
        ,[IdCardNumber]
        ,[PassportNo]
        ,[PassportExp]
        ,[GeoMapLatitude]
        ,[GeoMapLongtitude]
        ,[DistanceToCompany]
        ,[EmployeeCard]
        ,[IssuedDate]
        ,[IssuedPlace]
		,[PlaceOfBirth]
		,[ContactName]
		,[UserRelationship]
		,[ContactPhone]
		,[ContactAddress]
        ,LRExamDate
        ,Listening
        ,Reading
        ,Speaking
        ,Writing
        ,[GroupId]
        ,g.Code
        ,g.NAME
        ,g.[Rank]
        ,e.JobTitleId
        ,jt.[Name]
    FROM (
        SELECT row_number() OVER (
                ORDER BY NickName ASC
                ) AS RowNumber
            ,em.[Id]
            ,em.[Code]
            ,[DomainUsername]
            ,[FullName]
            ,[NickName]
            ,[DOB]
            ,[Email]
            ,[Enabled]
            ,[Phone]
            ,[Gender]
            ,[Roles]
            ,GroupId
			,[GroupDate]
            ,JobTitleId
            ,[JoinedDate]
            ,[ConfirmedDate]
            ,[ResignedDate]
            ,[Skype]
            ,[Yahoo]
            ,[Marriage]
            ,em.[CreatedDate]
            ,em.[CreatedBy]
            ,em.[ModifiedDate]
            ,em.[ModifiedBy]
            ,[isDeleted]
            ,[PermanentAddress]
            ,[CurrentAddress]
            ,[IdCardNumber]
            ,[PassportNo]
            ,[PassportExp]
            ,[GeoMapLatitude]
            ,[GeoMapLongtitude]
            ,[DistanceToCompany]
            ,[EmployeeCard]
            ,[IssuedDate]
            ,[IssuedPlace]
			,[PlaceOfBirth]
			,[ContactName]
			,[UserRelationship]
			,[ContactPhone]
			,[ContactAddress]
			,[SocialInsuranceNo]
        FROM dbo.Employee AS em WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS emg WITH (NOLOCK) ON em.GroupId = emg.ID
        WHERE 
            (IsSpecialUser = @isSpecial
				OR @isSpecial IS NULL)
			AND (@isDelete = 1
						AND ISNULL(isDeleted, 0) = 1 
			OR (

						@isDelete = 0
						AND ISNULL(isDeleted, 0) = 0 
						AND
						(
							[Enabled] <> @isDisable
						)
					
				)
				)
            AND (
                @query IS NULL
                OR NickName LIKE @q
                OR FullName LIKE @q
                OR DomainUsername LIKE @q
                OR em.Code LIKE @q
                )
            AND (
                @groupId IS NULL
                OR GroupId = @groupId
                )
            AND (
                @jobTitleId IS NULL
                OR JobTitleId = @jobTitleId
                )
            AND (
                @departmentId IS NULL
                OR emg.DepartmentId = @departmentId
                )
        ) AS e
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN dbo.Department AS d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
        LEFT JOIN HRM.dbo.TOEIC AS t WITH (NOLOCK) ON e.Id = t.EmpId
		LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.JobGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeJobGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.JobGrade as jg WITH (NOLOCK) on ejg.JobGradeId = jg.JobGradeId
			  WHERE ejg.IsCurrentUsed = 1
			  AND ISNULL(ejg.IsDeleted,0) = 0
		) AS gra ON e.Id = gra.EmpId
		LEFT JOIN (
			SELECT emg.EmpId
			,emg.AffectedDate
			,emg.Note
			,mg.ManagementGradeId
			,mg.Code
			,mg.Name
			,mg.Description
			  FROM HRM.dbo.EmployeeManagementGrade as emg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.ManagementGrade as mg WITH (NOLOCK) on emg.ManagementGradeId = mg.ManagementGradeId
			  WHERE emg.IsCurrentUsed = 1
			  AND ISNULL(emg.IsDeleted,0) = 0
		) AS mgra ON e.Id = mgra.EmpId
    WHERE RowNumber BETWEEN (@pageNumber - 1) * @pageSize
            AND @pageNumber * @pageSize
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate

END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetGeoMapEmployee] @query NVARCHAR(256)
AS
/*
Created: 20230519@Quinn.Vo
Task: Get geo info of employee.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @q NVARCHAR(256)
    SET @q = '%' + @query + '%'

    IF LEN(@query) = 0
        SET @query = NULL

    SELECT e.[Id]
        ,e.[Code]
        ,[FullName]
        ,[NickName]
		,e.GeoMapLongtitude
		,e.GeoMapLatitude
        FROM Employee AS e WITH (NOLOCK)
        WHERE 
			e.Enabled = 1
			AND e.isDeleted = 0
            AND (
                @query IS NULL
                OR e.NickName LIKE @q
                OR e.FullName LIKE @q
                OR e.DomainUsername LIKE @q
                OR e.Code LIKE @q
                )
END

GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetGroupHandleRequest]	
	@id int
AS
/*
Created: 2012/05/22@Dan
Task: get group that handle request of an employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SELECT g.*
	FROM dbo.EmployeeGroup_Employee eg 
			LEFT JOIN dbo.EmployeeGroup g ON eg.Groupid=g.ID 					
	WHERE eg.EmpID=@id AND ISNULL(HandleRequest,0) = 1		
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetHR]
AS
	/*
	Created: 
	Task: get HR for Nex request.
	DB: NexHR
	Original: 

	Revisions:    
		20250507@Henry.Dinh: update nickname
	Param's Explanation:
	*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT e.ID
		,e.NickName
		,e.FullName
		,e.roles
		,e.GroupID
		,e.Email
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE e.NickName IN ('Maria', 'Hoa')
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetLeaderForStanbyTeam]
AS
/*
Created: 20150814@Quinn.Vo
Task: Get leaders which approved check in/out request for Stanby.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.GroupID
        ,e.Email
		,con.ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
	LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
    WHERE NickName = 'Viet'
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetListStaffInfoNexHour] @ExcelData NVARCHAR(MAX)
AS
/*
Created: 20240702@Brian.Nguyen
Task: Get list staff info by conditions.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	-- Parse data from @ExcelData to temp table #tmpEmployee
    DECLARE @iDoc INT
    EXECUTE sys.sp_xml_preparedocument @iDoc OUTPUT, @ExcelData

    CREATE TABLE #tmpEmployee (
		NickName    NVARCHAR(20),
		Duration	NUMERIC(18,1),
		[Date]		DATETIME, 
		Notes		NVARCHAR(250),
		[Unique]		NVARCHAR(200)
    )

    INSERT INTO #tmpEmployee (
        NickName,
        Duration,
		[Date],
		Notes,
		[Unique]
    )
    SELECT ox.NickName,
           ox.Duration,
           ox.Date,
		   ox.Notes,
		   ox.[Unique]
    FROM OPENXML(@iDoc, '/ROOT/OvertimeExcelDataViewModel', 1)
    WITH (NickName NVARCHAR(20), Duration NUMERIC(18,1), [Date] DATETIME, Notes NVARCHAR(250), [Unique] NVARCHAR(200)) AS ox

	
	SELECT e.[Id]
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,e.[NickName]
        ,[DOB] AS DateOfBirth
        ,[Email]
        ,[Phone]
        ,[JoinedDate]
		,[GroupDate]
        ,[Skype]
        ,g.Code AS TeamCode
        ,g.NAME AS TeamName
        ,jt.NAME AS JobTitle
		,CASE WHEN EXISTS(
							SELECT 1 
							FROM dbo.[EmployeeCalendar] WITH (NOLOCK)
							WHERE DATEDIFF(DAY, tmpN.[Date], WorkDate) = 0
							AND ( IsOffDay = 1 OR IsHoliday = 1) 
							AND EmpId =  e.[Id]
						)
			   THEN
				   0
			   ELSE
				   tmpN.Duration*1.5
			   END  as NexHour
		,CASE WHEN EXISTS(
							SELECT 1 
							FROM dbo.[EmployeeCalendar] WITH (NOLOCK)
							WHERE DATEDIFF(DAY, tmpN.[Date], WorkDate) = 0
							AND ( IsOffDay = 1 OR IsHoliday = 1) 
							AND EmpId =  e.[Id]
						)
			   THEN
				   0
			   ELSE
				   1
			   END as IsWorkDay
		,tmpN.Duration AS [Hour]
		,tmpN.[Date]
		,tmpN.Notes
		,tmpN.[Unique]
        FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
		INNER JOIN #tmpEmployee AS tmpN WITH (NOLOCK) ON tmpN.NickName = e.NickName
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
        WHERE 
            e.IsSpecialUser = 0
			AND e.Enabled = 1
			AND e.isDeleted = 0 
		ORDER BY tmpN.[Date] ASC

	DROP TABLE #tmpEmployee
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetManagements]
AS
/*
  Created: 20150703@Tim.Trinh
  Task: get management that can handle unpaid leave request for employee. 
  DB: NexHR
  Original: 

  Revisions:  
    20250507@Henry.Dinh: update group id from 11 to 1081
  Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT e.Id AS EmpId
        ,e.NickName
        ,e.FullName
        ,e.Email
		,con.ConversationId
    FROM [dbo].[Employee] AS e WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
    WHERE e.GroupId = 1081
	AND e.isDeleted = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetMissedLogTime]
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
/*
Created: 20250228@Henry.dinh
Task: Insert/Update get missed logging 
DB: NexHRM
Original: Employee_GetMissedLogTime
Revisions:  
	- 20250307 - Added date range parameters and changed to use Date instead of Month/Year
				 Default date range is now first day to last day of current month
	- 20240319 - Removed RedmineNexcel dependencies as it's now handled by a separate API
Param's Explanation:
- @FromDate: Optional start date for filtering (defaults to first day of current month if NULL)
- @ToDate: Optional end date for filtering (defaults to last day of current month if NULL)
*/
BEGIN
    DECLARE @Yesterday DATE = 
        CASE 
            WHEN DATEPART(WEEKDAY, dbo.fnc_GetClientDateTime()) = 2 THEN
                DATEADD(DAY, -3, dbo.fnc_GetClientDateTime())
            ELSE
                DATEADD(DAY, -1, dbo.fnc_GetClientDateTime())
        END;
   
    DECLARE @FirstDayOfMonth DATE = DATEFROMPARTS(YEAR(dbo.fnc_GetClientDateTime()), MONTH(dbo.fnc_GetClientDateTime()), 1);
    DECLARE @LastDayOfMonth DATE = EOMONTH(dbo.fnc_GetClientDateTime());
    
    SET @FromDate = ISNULL(@FromDate, @FirstDayOfMonth);
    SET @ToDate = ISNULL(@ToDate, @LastDayOfMonth);
    
    WITH ExtractedDomainUsers AS (
        SELECT 
            Id,
            -- Extract username part after domain (e.g., "henry.dinh" from "spvn\henry.dinh")
            CASE 
                WHEN CHARINDEX('\', DomainUsername) > 0 
                THEN SUBSTRING(DomainUsername, CHARINDEX('\', DomainUsername) + 1, LEN(DomainUsername)) 
                ELSE DomainUsername 
            END AS ExtractedUsername,
            DomainUsername
        FROM [dbo].[Employee] WITH (NOLOCK)
        WHERE IsDeleted = 0 OR IsDeleted IS NULL
    ),
    ValidLeaders AS (
        -- Get leaders from GroupSupervisor
        SELECT DISTINCT gs.ApproverId
        FROM [dbo].[GroupSupervisor] gs WITH (NOLOCK)
        WHERE (gs.IsDeleted = 0 OR gs.IsDeleted IS NULL)
    ),
    EmployeesToExclude AS (
        -- Get emps is not Leader but exist in EmployeeSupervisor
        SELECT DISTINCT EmpId
        FROM [dbo].[EmployeeSupervisor] es WITH (NOLOCK)
        WHERE (es.IsDeleted = 0 OR es.IsDeleted IS NULL)
        AND es.EmpId NOT IN (SELECT ApproverId FROM ValidLeaders)
        UNION
        SELECT DISTINCT ApproverId
        FROM [dbo].[EmployeeSupervisor] es WITH (NOLOCK)
        WHERE (es.IsDeleted = 0 OR es.IsDeleted IS NULL)
        AND es.ApproverId NOT IN (SELECT ApproverId FROM ValidLeaders)
    ),
    MissedCounts AS (
        SELECT 
            EmpID,
            COUNT(*) AS MissedCount
        FROM [dbo].[EmployeeLogTimeMissed] WITH (NOLOCK)
        WHERE [Date] BETWEEN @FromDate AND @ToDate
        GROUP BY EmpID
    )
    SELECT DISTINCT
        eg.Code AS TeamCode,
        e.Id AS EmpID,
        e.NickName,
        CASE 
            WHEN e.Id = gs.ApproverId THEN 1 ELSE 0
        END AS IsLeader,
        ISNULL(mc.MissedCount, 0) AS MissedCount,
		jt.Name As JobTitle,
        ec.ConversationId,
        edu.DomainUsername
    FROM [dbo].[EmployeeGroup] eg WITH (NOLOCK)
    INNER JOIN [dbo].[Employee] e WITH (NOLOCK) ON eg.Id = e.GroupId
    LEFT JOIN [dbo].[GroupSupervisor] gs WITH (NOLOCK) ON eg.Id = gs.GroupId AND (gs.IsDeleted = 0 OR gs.IsDeleted IS NULL)
    LEFT JOIN [dbo].[EmployeeSupervisor] es WITH (NOLOCK) ON e.Id = es.EmpId AND (es.IsDeleted = 0 OR es.IsDeleted IS NULL)
	LEFT JOIN [HRM].[dbo].[JobTitle] jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
    LEFT JOIN ExtractedDomainUsers edu ON e.Id = edu.Id
    LEFT JOIN MissedCounts mc ON e.Id = mc.EmpID
    LEFT JOIN [dbo].[EmployeeConversation] ec WITH (NOLOCK) ON e.Id = ec.EmpId
    LEFT JOIN [dbo].[EmployeeAttendance] ea WITH (NOLOCK)
        ON e.Id = ea.EmpId 
        AND ea.AttendanceDate = @Yesterday
    WHERE 
        (eg.Deleted = 0 OR eg.Deleted IS NULL)
        AND (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
        AND e.Enabled = 1
        AND (
            e.Id = gs.ApproverId
            OR (ea.InFixed IS NOT NULL AND (ea.LeaveDay != 1 OR ea.LeaveDay IS NULL))
        )
        -- Exclude EmpId is not Leader but exist in EmployeeSupervisor
        AND e.Id NOT IN (SELECT EmpId FROM EmployeesToExclude)
    ORDER BY 
        eg.Code, 
        IsLeader DESC, 
        e.NickName;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetSkypeAccount]
	@EmployeeId INT = NULL
	,@GroupId INT = NULL
	,@IsConfirmed INT = NULL
AS
/*
Created: 20180725@Oliver.Dang
Task: Get all Employee with Skype account map from Nexbot
DB: NexHR & NexBot
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT u.ConversationId
		  ,e.Id AS EmpId
		  ,e.Code 
		  ,e.FullName
		  ,e.NickName
		  ,g.Id AS GroupId
		  ,g.Code
		  ,g.Name
	FROM [dbo].Employee AS e WITH (NOLOCK)
	LEFT JOIN [dbo].[EmployeeGroup] AS g WITH (NOLOCK) ON e.GroupId = g.Id
	LEFT JOIN [dbo].EmployeeConversation AS u WITH (NOLOCK) ON e.iD = u.EmpId
	WHERE e.[Enabled] = 1 AND e.isDeleted = 0
		AND (@EmployeeId IS NULL OR @EmployeeId < 0 OR e.Id = @EmployeeId)
		AND (@GroupId IS NULL OR g.Id = @GroupId)
		AND (@IsConfirmed IS NULL
			 OR ((CASE WHEN u.EmpId IS NULL THEN 0 ELSE 1 END) = @IsConfirmed))
	ORDER BY g.[Rank]
			,g.Code
			,e.Code
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetStaffInfo] @query NVARCHAR(256)
    ,@groupId INT = NULL
	,@isSpecial BIT = NULL
	,@departmentId INT = NULL
AS
/*
Created: 20230519@Quinn.Vo
Task: Get staff info by conditions.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
@departmentId - Optional parameter to filter by department
*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @q NVARCHAR(256)
    SET @q = '%' + @query + '%'

    IF LEN(@query) = 0
        SET @query = NULL

    SELECT e.[Id]
        ,e.[Code]
        ,[DomainUsername]
        ,[FullName]
        ,[NickName]
        ,[DOB] AS DateOfBirth
        ,[Email]
        ,[Phone]
        ,[JoinedDate]
		,[GroupDate]
        ,[Skype]
        ,g.Code AS TeamCode
        ,g.NAME AS TeamName
        ,jt.NAME AS JobTitle
        ,d.Name AS DepartmentName
        FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
        LEFT JOIN dbo.Department AS d WITH (NOLOCK) ON g.DepartmentId = d.DepartmentId
        WHERE 
            (e.IsSpecialUser = @isSpecial
				OR @isSpecial IS NULL)
			AND e.Enabled = 1
			AND e.isDeleted = 0
            AND (
                @query IS NULL
                OR e.NickName LIKE @q
                OR e.FullName LIKE @q
                OR e.DomainUsername LIKE @q
                OR e.Code LIKE @q
                )
            AND (
                @groupId IS NULL
                OR GroupId = @groupId
                )
            AND (
                @departmentId IS NULL
                OR g.DepartmentId = @departmentId
                )
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetSubTeam]
	@empId int
AS
BEGIN
	SET NOCOUNT ON;

	select g.* 
	from dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
		JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON eg.GroupID = g.ID AND isMaingroup=0
	where EmpID=@empId 
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetSupervisorForCheckInOut] 
	@employeeId INT
	,@IsStandbyAtWeekend BIT 
AS
/*
Created: 20240329@Quinn
Task: Get Supervisors for notification check in/out request.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @IsSupReadOnly BIT = 0
	DECLARE @LeaderOfStandbyTeam INT,
		@StandByTeam VARCHAR(50)
	SELECT @LeaderOfStandbyTeam = e.Id FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON e.Id = st.Value AND st.Name = 'LeaderOfStandbyTeam'

	SELECT @StandByTeam = st.Value FROM dbo.SystemSetting AS st WITH (NOLOCK) WHERE st.Name = 'StanbyTeam'

	CREATE TABLE #tempSupIds(
		SupervisorId INT,
		IsReadOnly BIT
	)

	IF (@IsStandbyAtWeekend = 1 AND 
		EXISTS (SELECT 1 FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id 
		WHERE e.Id = @employeeId
			AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ',')))
	)
	BEGIN
		INSERT INTO #tempSupIds(SupervisorId, IsReadOnly)
		SELECT e.Id, 0
		FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS eg ON e.GroupId = eg.Id
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
		WHERE e.Id = @LeaderOfStandbyTeam
		AND e.isDeleted = 0
		SET @IsSupReadOnly = 1
	END
	ELSE
	BEGIN
		IF EXISTS (SELECT 1 FROM dbo.EmployeeSupervisor WITH (NOLOCK) WHERE EmpId = @employeeId)
		BEGIN
			INSERT INTO #tempSupIds(SupervisorId, IsReadOnly)
			SELECT DISTINCT sup.Id, @IsSupReadOnly
			FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			INNER JOIN dbo.Employee AS sup WITH (NOLOCK) 
				ON sup.Id = es.ApproverId AND sup.isDeleted = 0 AND sup.Enabled = 1
			WHERE es.EmpId = @employeeId 
				AND ISNULL(es.IsDeleted, 0) = 0
				AND es.ApproverId <> @employeeId
			AND NOT EXISTS (
					SELECT 1 FROM #tempSupIds WHERE SupervisorId = es.ApproverId
			)
		END
		ELSE
		BEGIN
			INSERT INTO #tempSupIds(SupervisorId, IsReadOnly)
			SELECT DISTINCT gs.ApproverId, @IsSupReadOnly
			FROM dbo.Employee AS e WITH (NOLOCK)
			INNER JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) 
				ON e.GroupId = gs.GroupId
			INNER JOIN dbo.Employee AS sup WITH (NOLOCK) 
				ON sup.Id = gs.ApproverId 
				AND sup.isDeleted = 0 
				AND sup.Enabled = 1
				AND ISNULL(gs.IsDeleted, 0) = 0
			WHERE e.Id = @employeeId
				AND gs.ApproverId <> @employeeId
				AND NOT EXISTS (
						SELECT 1 FROM #tempSupIds WHERE SupervisorId = gs.ApproverId
				)
		END
	END

	SELECT DISTINCT 
		e.EmpId
		,e.NickName
		,e.FullName
		,e.Code
		,sup.SupervisorId
		,sup.SupervisorNickName
		,sup.SupervisorFullName
		,sup.SupervisorEmail
		,sup.SupervisorConversationId
		,sup.IsReadOnly
	FROM (
			SELECT Id AS EmpId
			,NickName
			,FullName
			,Code 
			FROM dbo.Employee WITH (NOLOCK)
			WHERE Id = @employeeId
		) AS e
		INNER JOIN
		(
			SELECT a.Id AS SupervisorId
				,NickName AS SupervisorNickName
				,FullName AS SupervisorFullName
				,Email AS SupervisorEmail 
				,con.ConversationId AS SupervisorConversationId
				,tsup.IsReadOnly
			FROM dbo.Employee AS a WITH (NOLOCK)
			INNER JOIN #tempSupIds tsup ON a.Id = tsup.SupervisorId
			LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON a.Id = con.EmpId
		) AS sup ON 1 = 1
		WHERE e.EmpId <> sup.SupervisorId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_GetSupervisorForLeave] 
	@employeeId INT
	,@IsManagementApprove BIT 
AS
/*
	Created: 20240329@Quinn
	Task: Get Supervisors for notification check leave request.
	DB: NexHR
	Original: 

	Revisions:
		- 20250327@Henry: Update MGT Code -> MGT/COR code
	Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @IsSupReadOnly BIT = 0
	CREATE TABLE #tempSupIds(
		SupervisorId INT,
		IsReadOnly BIT
	)

	IF (@IsManagementApprove = 1)
	BEGIN
		INSERT INTO #tempSupIds(SupervisorId, IsReadOnly)
		SELECT e.Id, 0
		FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS eg ON e.GroupId = eg.Id
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
		WHERE eg.Code IN ('COR', 'MGT')
		AND e.isDeleted = 0
		SET @IsSupReadOnly = 1
	END

	IF EXISTS (SELECT 1 FROM dbo.EmployeeSupervisor WITH (NOLOCK) WHERE EmpId = @employeeId)
	BEGIN
		INSERT INTO #tempSupIds(SupervisorId, IsReadOnly)
		SELECT DISTINCT sup.Id, @IsSupReadOnly
		FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		INNER JOIN dbo.Employee AS sup WITH (NOLOCK) 
			ON sup.Id = es.ApproverId AND sup.isDeleted = 0 AND sup.Enabled = 1
		WHERE es.EmpId = @employeeId 
			AND ISNULL(es.IsDeleted, 0) = 0
			AND es.ApproverId <> @employeeId
		AND NOT EXISTS (
				SELECT 1 FROM #tempSupIds WHERE SupervisorId = es.ApproverId
		)
	END
	ELSE
	BEGIN
		INSERT INTO #tempSupIds(SupervisorId, IsReadOnly)
		SELECT DISTINCT gs.ApproverId, @IsSupReadOnly
		FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) 
			ON e.GroupId = gs.GroupId
		INNER JOIN dbo.Employee AS sup WITH (NOLOCK) 
			ON sup.Id = gs.ApproverId 
			AND sup.isDeleted = 0 
			AND sup.Enabled = 1
			AND ISNULL(gs.IsDeleted, 0) = 0
		WHERE e.Id = @employeeId
			AND gs.ApproverId <> @employeeId
			AND NOT EXISTS (
					SELECT 1 FROM #tempSupIds WHERE SupervisorId = gs.ApproverId
			)
	END

	SELECT DISTINCT 
		e.EmpId
		,e.NickName
		,e.FullName
		,e.Code
		,sup.SupervisorId
        ,sup.SupervisorNickName
        ,sup.SupervisorFullName
        ,sup.SupervisorEmail
		,sup.SupervisorConversationId
		,sup.IsReadOnly
    FROM (
			SELECT Id AS EmpId
			,NickName
			,FullName
			,Code 
			FROM dbo.Employee WITH (NOLOCK)
			WHERE Id = @employeeId
		) AS e
		INNER JOIN
		(
			SELECT a.Id AS SupervisorId
				,NickName AS SupervisorNickName
				,FullName AS SupervisorFullName
				,Email AS SupervisorEmail 
				,con.ConversationId AS SupervisorConversationId
				,tsup.IsReadOnly
			FROM dbo.Employee AS a WITH (NOLOCK)
			INNER JOIN #tempSupIds tsup ON a.Id = tsup.SupervisorId
			LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON a.Id = con.EmpId
		) AS sup ON 1 = 1
		WHERE e.EmpId <> sup.SupervisorId

	DROP TABLE #tempSupIds
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_Insert] @fullName NVARCHAR(50)
    ,@nickName NVARCHAR(20)
    ,@domainUserName VARCHAR(50)
    ,@code VARCHAR(20)
    ,@password NVARCHAR(100)
    ,@monShift INT = NULL
    ,@tueShift INT = NULL
    ,@wedShift INT = NULL
    ,@thuShift INT = NULL
    ,@friShift INT = NULL
    ,@satShift INT = NULL
    ,@sunShift INT = NULL
    ,@groupID INT
    ,@createBy INT
    ,@employeeId INT OUTPUT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: insert an employee
DB: NexHRM
Original: 

Revisions:    
    20250507@Henry.Dinh: insert password
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @CompanyId INT
	SELECT @CompanyId = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId'

    IF EXISTS (
            SELECT 1
            FROM dbo.Employee WITH (NOLOCK)
            WHERE Code = @code
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This user has already existed !'

        RETURN
    END

    --SET XACT_ABORT ON;

    --BEGIN TRY
    --    BEGIN TRANSACTION;

        INSERT INTO HRM.dbo.Employee (
            Code
            ,FullName
            ,NickName
            ,DOB
            ,CreatedDate
            ,CreatedBy
            )
        VALUES (
            @code
            ,@fullName
            ,@nickName
            ,CAST('1-1-1990' AS SMALLDATETIME)
            ,dbo.fnc_GetClientDateTime()
            ,@createBy
            )

        SET @employeeId = CAST(SCOPE_IDENTITY() AS INT)

		INSERT INTO HRM.dbo.EmployeeCompany (EmpId, CompanyId)
		VALUES (@employeeId, @CompanyId)

		INSERT INTO dbo.EmployeeExt (
			Id
            ,DomainUsername
            ,Password
            ,GroupID
			,Enabled
			,IsSpecialUser
            )
        VALUES (
			@employeeId
            ,@domainUsername
            ,@password
            ,@groupID
			,0
			,0
            )

        INSERT INTO EmployeeShift (
            [EmpID]
            ,[MonShift]
            ,[TueShift]
            ,[WedShift]
            ,[ThuShift]
            ,[FriShift]
            ,[SatShift]
            ,[SunShift]
            )
        VALUES (
            @employeeId
            ,@monShift
            ,@tueShift
            ,@wedShift
            ,@thuShift
            ,@friShift
            ,@satShift
            ,@sunShift
            )

        INSERT INTO dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        VALUES (
            @groupID
            ,@employeeId
            ,1
            )

        DECLARE @fromdate DATETIME
            ,@todate DATETIME

        SET @fromdate = dbo.fnc_GetClientDateTime()
		SELECT @todate = MAX(WorkDate) FROM dbo.EmployeeCalendar WITH (NOLOCK)

        EXECUTE EmployeeCalendar_Insert @employeeId
            ,@fromdate
            ,@todate

        SELECT @ErrCode = 0
            ,@ErrMsg = ''

    --    COMMIT TRANSACTION;
    --END TRY

    --BEGIN CATCH
    --    ROLLBACK TRANSACTION

    --    SELECT @ErrCode = 1
    --        ,@ErrMsg = N'Insert Employee unsuccessfully.'
    --END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_Transfer] @EmployeeId INT
	,@LastWorkingDate DATE
	,@AffectedDate DATE
	,@ToGroupId INT
	,@ShiftId INT
	,@IsTransferLateInEarlyOut BIT
	,@IsTransferAnnualLeaveBalance BIT
	,@IsTransferNexBalance BIT
	,@IsSpecialUser BIT = 0
	,@ModifiedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210901@Quinn.Vo
Task: transfer employee
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @FromDomain VARCHAR(20)
		,@ToDomain VARCHAR(20)
		,@FromDomainEmail VARCHAR(20)
		,@ToDomainEmail VARCHAR(20)
		,@FromCompanyId INT
		,@ToCompanyId INT
		,@Todate DATETIME
		,@NexBalance INT = 0
		
	DECLARE @CreditDays NUMERIC(18, 1)
		,@LateInEarlyOut INT

	SELECT @FromDomain = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
	SELECT @FromDomainEmail = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'
	SELECT @FromCompanyId = [Value] FROM dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'CompanyId'

	IF (@FromCompanyId = 1)
	BEGIN
		SET @ToCompanyId = 2
		SELECT @ToDomain = [Value] FROM LumiHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
		SELECT @ToDomainEmail = [Value] FROM LumiHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'

		IF EXISTS (SELECT 1 FROM LumiHRM.dbo.EmployeeExt WITH (NOLOCK) WHERE Id = @EmployeeId)
		BEGIN
			UPDATE a WITH (UPDLOCK, ROWLOCK)
				SET a.DomainUsername = @ToDomain + REPLACE(a.DomainUsername, @FromDomain, '') 
				,a.Email = REPLACE(a.DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,a.IsSpecialUser = @IsSpecialUser
				,a.IsDeleted = 0
				,a.Enabled = 1
				,a.GroupId = @ToGroupId
				,a.Roles = b.Roles
				,a.JoinedDate = @AffectedDate
				,a.ConfirmedDate = @AffectedDate
			FROM LumiHRM.dbo.EmployeeExt AS a 
			INNER JOIN dbo.EmployeeExt AS b ON a.Id = b.Id 
			WHERE a.Id = @employeeId
		END
		ELSE
		BEGIN
			INSERT INTO LumiHRM.dbo.EmployeeExt (
				Id
				,DomainUsername
				,Email
				,IsSpecialUser
				,IsDeleted
				,Enabled
				,GroupID
				,Roles
				,JoinedDate
				,ConfirmedDate
			)
			SELECT 
				@EmployeeId
				,@ToDomain + REPLACE(DomainUsername, @FromDomain, '')
				,REPLACE(DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,@IsSpecialUser
				,0
				,1
				,@ToGroupId
				,Roles
				,@AffectedDate
				,@AffectedDate
			FROM dbo.EmployeeExt WITH (NOLOCK)
			WHERE Id = @EmployeeId
		END

		INSERT INTO LumiHRM.dbo.EmployeeShift (
            [EmpID]
            ,[MonShift]
            ,[TueShift]
            ,[WedShift]
            ,[ThuShift]
            ,[FriShift]
            )
        VALUES (
            @EmployeeId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            )

        INSERT INTO LumiHRM.dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        VALUES (
            @ToGroupId
            ,@EmployeeId
            ,1
            )

        SET @todate = DATEADD(yy, DATEDIFF(yy, 0, dbo.fnc_GetClientDateTime()) + 1, -1)

        EXECUTE LumiHRM.dbo.EmployeeCalendar_Insert @EmployeeId
            ,@AffectedDate
            ,@todate

		-- AL
		IF(@IsTransferAnnualLeaveBalance = 1)
		BEGIN
			SELECT @CreditDays = SUM(ISNULL (CreditDays, 0)) - SUM(ISNULL(DeductDays, 0))
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmployeeId 
				AND YEAR(FromDate) = YEAR(@AffectedDate) 
				AND ISNULL(Deleted, 0) <> 1

			INSERT INTO LumiHRM.dbo.LeaveTrans (
				EmpId
				,Description
				,FromDate
				,ToDate
				,DeductDays
				,CreditDays
				,CreatedDate
				)
			VALUES (
				@EmployeeId
				,'Init'
				,@AffectedDate
				,@ToDate
				,0
				,@CreditDays
				,dbo.fnc_GetClientDateTime()
				)
		END

		-- Late In/Early Out
		IF (@IsTransferLateInEarlyOut = 1)
		BEGIN
			SELECT @LateInEarlyOut = [Minutes]
			FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
			WHERE EmpId = @EmployeeId
			AND [Year] = YEAR(@AffectedDate)

			IF (@LateInEarlyOut IS NOT NULL)
			BEGIN
				INSERT INTO LumiHRM.dbo.LateInEarlyOutTrans(
					[EmpId]
				  ,[WorkDate]
				  ,[Type]
				  ,[Minutes]
				  ,[ModifiedDate]
				  ,[IsManual]
				)
				VALUES(@EmployeeId, @AffectedDate, 2, @LateInEarlyOut, dbo.fnc_GetClientDateTime(), 1)
			END
			
			INSERT INTO LumiHRM.dbo.LateInEarlyOutYearly(
				[EmpId]
				  ,[Year]
				  ,[Minutes]
				  ,[ModifiedDate]
			)
			VALUES(@EmployeeId, YEAR(@AffectedDate), ISNULL(@LateInEarlyOut, 0), dbo.fnc_GetClientDateTime())

		END

		IF (@IsTransferNexBalance = 1)
		BEGIN
			SELECT @NexBalance = CASE 
					WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
					ELSE 0
					END
			FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
				AND oty.[Year] = YEAR(@AffectedDate)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
				AND la.[Year] = YEAR(@AffectedDate)
			WHERE e.Id = @EmployeeId

			IF (@NexBalance <> 0)
			BEGIN
				INSERT INTO LumiHRM.dbo.OvertimeTrans(
							EmpId
							,[Date]
							,[Minutes]
							,[Type]
							,ModifiedBy
							,ModifiedDate
							)
							SELECT 
							@EmployeeId
							,@AffectedDate
							,@NexBalance
							,1
							,@ModifiedBy
							,dbo.fnc_GetClientDateTime()
			END

			INSERT INTO LumiHRM.dbo.OvertimeYearly (
					EmpId
					,[Year]
					,[Minutes]
					,[ModifiedBy]
					,[ModifiedDate]
					)
				VALUES(@EmployeeId
					,YEAR(@AffectedDate)
					,@NexBalance
					,@ModifiedBy
					,dbo.fnc_GetClientDateTime()
				)
		END
	END
	ELSE IF (@FromCompanyId = 2)
	BEGIN
		SET @ToCompanyId = 1
		SELECT @ToDomain = [Value] FROM NexHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainName'
		SELECT @ToDomainEmail = [Value] FROM NexHRM.dbo.SystemSetting WITH (NOLOCK) WHERE [Name] = 'DomainEmail'

		IF EXISTS (SELECT 1 FROM NexHRM.dbo.EmployeeExt WITH (NOLOCK) WHERE Id = @EmployeeId)
		BEGIN
			UPDATE a WITH (UPDLOCK, ROWLOCK)
				SET a.DomainUsername = @ToDomain + REPLACE(a.DomainUsername, @FromDomain, '') 
				,a.Email = REPLACE(a.DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,a.IsSpecialUser = @IsSpecialUser
				,a.IsDeleted = 0
				,a.Enabled = 1
				,a.GroupId = @ToGroupId
				,a.Roles = b.Roles
				,a.JoinedDate = @AffectedDate
				,a.ConfirmedDate = @AffectedDate
			FROM NexHRM.dbo.EmployeeExt AS a 
			INNER JOIN dbo.EmployeeExt AS b ON a.Id = b.Id 
			WHERE a.Id = @employeeId
		END
		ELSE
		BEGIN
			INSERT INTO NexHRM.dbo.EmployeeExt (
				Id
				,DomainUsername
				,Email
				,IsSpecialUser
				,IsDeleted
				,Enabled
				,GroupID
				,Roles
				,JoinedDate
				,ConfirmedDate
			)
			SELECT 
				@EmployeeId
				,@ToDomain + REPLACE(DomainUsername, @FromDomain, '')
				,REPLACE(DomainUsername, @FromDomain, '') + '@' + @ToDomainEmail
				,@IsSpecialUser
				,0
				,1
				,@ToGroupId
				,Roles
				,@AffectedDate
				,@AffectedDate
			FROM dbo.EmployeeExt WITH (NOLOCK)
			WHERE Id = @EmployeeId
		END

		INSERT INTO NexHRM.dbo.EmployeeShift (
            [EmpID]
            ,[MonShift]
            ,[TueShift]
            ,[WedShift]
            ,[ThuShift]
            ,[FriShift]
            )
        VALUES (
            @EmployeeId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            ,@ShiftId
            )

        INSERT INTO NexHRM.dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        VALUES (
            @ToGroupId
            ,@EmployeeId
            ,1
            )

        SET @todate = DATEADD(yy, DATEDIFF(yy, 0, dbo.fnc_GetClientDateTime()) + 1, -1)

        EXECUTE NexHRM.dbo.EmployeeCalendar_Insert @EmployeeId
            ,@AffectedDate
            ,@todate

		-- AL
		IF(@IsTransferAnnualLeaveBalance = 1)
		BEGIN
			SELECT @CreditDays = SUM(ISNULL (CreditDays, 0)) - SUM(ISNULL(DeductDays, 0))
			FROM dbo.LeaveTrans WITH (NOLOCK)
			WHERE EmpId = @EmployeeId 
				AND YEAR(FromDate) = YEAR(@AffectedDate) 
				AND ISNULL(Deleted, 0) <> 1

			INSERT INTO NexHRM.dbo.LeaveTrans (
				EmpId
				,Description
				,FromDate
				,ToDate
				,DeductDays
				,CreditDays
				,CreatedDate
				)
			VALUES (
				@EmployeeId
				,'Init'
				,@AffectedDate
				,@ToDate
				,0
				,@CreditDays
				,dbo.fnc_GetClientDateTime()
				)
		END

		-- Late In/Early Out
		IF (@IsTransferLateInEarlyOut = 1)
		BEGIN
			SELECT @LateInEarlyOut = [Minutes]
			FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
			WHERE EmpId = @EmployeeId
			AND [Year] = YEAR(@AffectedDate)

			IF (@LateInEarlyOut IS NOT NULL)
			BEGIN
				INSERT INTO NexHRM.dbo.LateInEarlyOutTrans(
					[EmpId]
				  ,[WorkDate]
				  ,[Type]
				  ,[Minutes]
				  ,[ModifiedDate]
				  ,[IsManual]
				)
				VALUES(@EmployeeId, @AffectedDate, 2, @LateInEarlyOut, dbo.fnc_GetClientDateTime(), 1)
			END
			
			INSERT INTO NexHRM.dbo.LateInEarlyOutYearly(
				[EmpId]
				  ,[Year]
				  ,[Minutes]
				  ,[ModifiedDate]
			)
			VALUES(@EmployeeId, YEAR(@AffectedDate), ISNULL(@LateInEarlyOut, 0), dbo.fnc_GetClientDateTime())

		END

		IF (@IsTransferNexBalance = 1)
		BEGIN
			SELECT @NexBalance = CASE 
					WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
					ELSE 0
					END
			FROM dbo.Employee AS e WITH (NOLOCK)
				LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
				AND oty.[Year] = YEAR(@AffectedDate)
				LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
				AND la.[Year] = YEAR(@AffectedDate)
			WHERE e.Id = @EmployeeId

			IF (@NexBalance <> 0)
			BEGIN
				INSERT INTO NexHRM.dbo.OvertimeTrans(
							EmpId
							,[Date]
							,[Minutes]
							,[Type]
							,ModifiedBy
							,ModifiedDate
							)
							SELECT 
							@EmployeeId
							,@AffectedDate
							,@NexBalance
							,1
							,@ModifiedBy
							,dbo.fnc_GetClientDateTime()
			END

			INSERT INTO NexHRM.dbo.OvertimeYearly (
					EmpId
					,[Year]
					,[Minutes]
					,[ModifiedBy]
					,[ModifiedDate]
					)
				VALUES(@EmployeeId
					,YEAR(@AffectedDate)
					,@NexBalance
					,@ModifiedBy
					,dbo.fnc_GetClientDateTime()
				)
		END
	END

	IF NOT EXISTS (SELECT 1 FROM HRM.dbo.EmployeeCompany WITH (NOLOCK) WHERE EmpId = @EmployeeId AND CompanyId = @ToCompanyId)
	BEGIN
		INSERT INTO HRM.dbo.EmployeeCompany(EmpId, CompanyId)
		VALUES (@EmployeeId, @ToCompanyId)
	END

	UPDATE dbo.EmployeeExt WITH (ROWLOCK, UPDLOCK)
	SET [Enabled] = 0
		,ResignedDate = @LastWorkingDate
	WHERE Id = @EmployeeId

	UPDATE HRM.dbo.Employee WITH (ROWLOCK, UPDLOCK)
	SET ModifiedDate = dbo.fnc_GetClientDateTime()
		,ModifiedBy = @ModifiedBy
	WHERE Id = @EmployeeId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Employee_UpdateEmployee] @employeeId INT
    ,@code VARCHAR(20) = NULL
    ,@domainUsername VARCHAR(50) = NULL
    ,@fullName NVARCHAR(50) = NULL
    ,@nickName NVARCHAR(20) = NULL
    ,@EducationBackground NVARCHAR(500) = NULL
    ,@EmployeeCard VARCHAR(11) = ''
    ,@dob DATETIME = NULL
    ,@email VARCHAR(50) = NULL
    ,@jobTitleId INT = NULL
    ,@enabled BIT = NULL
	,@isSpecialUser BIT = NULL
    ,@phone VARCHAR(50) = NULL
    ,@gender TINYINT = NULL
    ,@mainGroupID INT = NULL
    ,@subGroups VARCHAR(50) = NULL
    ,@Roles VARCHAR(20) = NULL
    ,@joinedDate DATETIME = NULL
    ,@confirmedDate DATETIME = NULL
    ,@resignedDate DATETIME = NULL
    ,@skype VARCHAR(100) = NULL
    ,@yahoo VARCHAR(100) = NULL
    ,@marriage BIT = NULL
    ,@address NVARCHAR(250) = NULL
    ,@currAddress NVARCHAR(250) = NULL
    ,@idCardNo NVARCHAR(20) = NULL
    ,@issuedDate DATETIME = NULL
    ,@issuedPlace NVARCHAR(250) = NULL
    ,@passportNo NVARCHAR(35) = NULL
    ,@passportExp DATETIME = NULL
	,@socialInsuranceNo VARCHAR(50) = NULL
    ,@geoMapLatitude VARCHAR(50) = ''
    ,@geoMapLongtitude VARCHAR(50) = ''
    ,@distanceToCompany VARCHAR(11) = ''
    ,@modifiedBy INT = NULL
	,@placeOfBirth NVARCHAR(20) = NULL
	,@contactName NVARCHAR(50) = NULL
	,@userRelationship NVARCHAR(11) = NULL
	,@contactPhone VARCHAR(50) = NULL
	,@contactAddress NVARCHAR(250) = NULL
	,@groupDate DATE = NULL
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20160218@Quinn.Vo
Task: update employee.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

	DECLARE 
		@logData NVARCHAR(MAX)
		,@xml XML
		,@Groups VARCHAR(50)

    BEGIN TRY
        BEGIN TRANSACTION;

		SELECT @Groups = 
			COALESCE(@Groups + ', ' + CAST(GroupId AS VARCHAR(10)), CAST(GroupId AS VARCHAR(10)))
		FROM dbo.EmployeeGroup_Employee 
		WHERE EmpID = @employeeId

		SELECT @logData = '<Employee_Old Code="'+ COALESCE(CAST(Code AS VARCHAR(50)),SPACE(1)) + '" ' +
		'DomainUsername="'+ COALESCE(DomainUsername,SPACE(1)) + '" ' +
		'FullName="'+ COALESCE(FullName,SPACE(1)) + '" ' +
		'NickName="'+ COALESCE(NickName,SPACE(1)) + '" ' +
		'EducationBackground="'+ COALESCE(EducationBackground,SPACE(1)) + '" ' +
		'DOB="'+ COALESCE(CAST(DOB AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Email="'+ COALESCE(Email,SPACE(1)) + '" ' +
		'JobTitleId="'+ COALESCE(CAST(JobTitleId AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Enabled="'+ COALESCE(CAST([Enabled] AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IsSpecialUser="'+ COALESCE(CAST(IsSpecialUser AS VARCHAR(4)),SPACE(1)) + '" ' +
		'Phone="'+ COALESCE(Phone,SPACE(1)) + '" ' +
		'Gender="'+ COALESCE(CAST(Gender AS VARCHAR(4)),SPACE(1)) + '" ' +
		'GroupId="'+ COALESCE(CAST(GroupId AS VARCHAR(50)),SPACE(1)) + '" ' +
		'Roles="'+ COALESCE([Roles],SPACE(1)) + '" ' +
		'JoinedDate="'+ COALESCE(CAST(JoinedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'GroupDate="'+ COALESCE(CAST(GroupDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ConfirmedDate="'+ COALESCE(CAST(ConfirmedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ResignedDate="'+ COALESCE(CAST(ResignedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Skype="'+ COALESCE(Skype,SPACE(1)) + '" ' +
		'Yahoo="'+ COALESCE(Yahoo,SPACE(1)) + '" ' +
		'Marriage="'+ COALESCE(CAST(Marriage AS VARCHAR(4)),SPACE(1)) + '" ' +
		'PermanentAddress="'+ COALESCE(PermanentAddress,SPACE(1)) + '" ' +
		'CurrentAddress="'+ COALESCE(CurrentAddress,SPACE(1)) + '" ' +
		'IdCardNumber="'+ COALESCE(IdCardNumber,SPACE(1)) + '" ' +
		'IssuedDate="'+ COALESCE(CAST(IssuedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IssuedPlace="'+ COALESCE(IssuedPlace,SPACE(1)) + '" ' +
		'PassportNo="'+ COALESCE(PassportNo,SPACE(1)) + '" ' +
		'PassportExp="'+ COALESCE(CAST(PassportExp AS VARCHAR(11)),SPACE(1)) + '" ' +
		'SocialInsuranceNo="'+ COALESCE(SocialInsuranceNo,SPACE(1)) + '" ' +
		'PlaceOfBirth="'+ COALESCE(PlaceOfBirth,SPACE(1)) + '" ' +
		'ContactName="'+ COALESCE(ContactName,SPACE(1)) + '" ' +
		'UserRelationship="'+ COALESCE(UserRelationship,SPACE(1)) + '" ' +
		'ContactPhone="'+ COALESCE(ContactPhone,SPACE(1)) + '" ' +
		'ContactAddress="'+ COALESCE(ContactAddress,SPACE(1)) + '" ' +
		'ModifiedBy="'+ COALESCE(CAST(ModifiedBy AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Groups="'+ COALESCE(@Groups,SPACE(1)) + '" />' + 

		'<Employee_new Code="'+ COALESCE(CAST(@code AS VARCHAR(50)),SPACE(1)) + '" ' +
		'DomainUsername="'+ COALESCE(@domainUsername,SPACE(1)) + '" ' +
		'FullName="'+ COALESCE(@fullName,SPACE(1)) + '" ' +
		'NickName="'+ COALESCE(@nickName,SPACE(1)) + '" ' +
		'EducationBackground="'+ COALESCE(@EducationBackground,SPACE(1)) + '" ' +
		'EmployeeCard="'+ COALESCE(@EmployeeCard,SPACE(1)) + '" ' +
		'DOB="'+ COALESCE(CAST(@dob AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Email="'+ COALESCE(@email,SPACE(1)) + '" ' +
		'JobTitleId="'+ COALESCE(CAST(@jobTitleId AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Enabled="'+ COALESCE(CAST(@enabled AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IsSpecialUser="'+ COALESCE(CAST(@isSpecialUser AS VARCHAR(4)),SPACE(1)) + '" ' +
		'Phone="'+ COALESCE(@phone,SPACE(1)) + '" ' +
		'Gender="'+ COALESCE(CAST(@gender AS VARCHAR(4)),SPACE(1)) + '" ' +
		'GroupId="'+ COALESCE(CAST(@mainGroupID AS VARCHAR(50)),SPACE(1)) + '" ' +

		'Roles="'+ COALESCE(@Roles,SPACE(1)) + '" ' +
		'JoinedDate="'+ COALESCE(CAST(@joinedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'GroupDate="'+ COALESCE(CAST(@groupDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ConfirmedDate="'+ COALESCE(CAST(@confirmedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'ResignedDate="'+ COALESCE(CAST(@resignedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'Skype="'+ COALESCE(@skype,SPACE(1)) + '" ' +
		'Yahoo="'+ COALESCE(@yahoo,SPACE(1)) + '" ' +
		'Marriage="'+ COALESCE(CAST(@marriage AS VARCHAR(4)),SPACE(1)) + '" ' +
		'PermanentAddress="'+ COALESCE(@address,SPACE(1)) + '" ' +
		'CurrentAddress="'+ COALESCE(@currAddress,SPACE(1)) + '" ' +
		'IdCardNumber="'+ COALESCE(@idCardNo,SPACE(1)) + '" ' +
		'IssuedDate="'+ COALESCE(CAST(@issuedDate AS VARCHAR(11)),SPACE(1)) + '" ' +
		'IssuedPlace="'+ COALESCE(@issuedPlace,SPACE(1)) + '" ' +
		'PassportNo="'+ COALESCE(@passportNo,SPACE(1)) + '" ' +
		'PassportExp="'+ COALESCE(CAST(@passportExp AS VARCHAR(11)),SPACE(1)) + '" ' +
		'SocialInsuranceNo="'+ COALESCE(@socialInsuranceNo,SPACE(1)) + '" ' +
		'PlaceOfBirth="'+ COALESCE(@placeOfBirth,SPACE(1)) + '" ' +
		'ContactName="'+ COALESCE(@contactName,SPACE(1)) + '" ' +
		'UserRelationship="'+ COALESCE(@userRelationship,SPACE(1)) + '" ' +
		'ContactPhone="'+ COALESCE(@contactPhone,SPACE(1)) + '" ' +
		'ContactAddress="'+ COALESCE(@contactAddress,SPACE(1)) + '" ' +
		'ModifiedBy="'+ COALESCE(CAST(@modifiedBy AS VARCHAR(11)),SPACE(1)) + '" />'

		FROM dbo.Employee WITH (NOLOCK)
		WHERE Id = @employeeId

        UPDATE HRM.dbo.Employee WITH (UPDLOCK, ROWLOCK)
        SET Code = @code
            ,FullName = @fullName
            ,NickName = @nickName
            ,EmployeeCard = @EmployeeCard
            ,DOB = @dob
            ,JobTitleId = @jobTitleId
            ,Phone = @phone
            ,Gender = @gender
			,GroupDate = @groupDate
            ,Skype = @skype
            ,Yahoo = @yahoo
            ,Marriage = @marriage
            ,PermanentAddress = @address
            ,CurrentAddress = @currAddress
            ,IdCardNumber = @idCardNo
            ,IssuedDate = @issuedDate
            ,IssuedPlace = @issuedPlace
            ,PassportNo = @passportNo
            ,PassportExp = @passportExp
			,SocialInsuranceNo = @socialInsuranceNo
            ,GeoMapLatitude = @geoMapLatitude
            ,GeoMapLongtitude = @geoMapLongtitude
            ,DistanceToCompany = @distanceToCompany
			,PlaceOfBirth = @placeOfBirth
			,ContactName = @contactName
			,UserRelationship = @userRelationship
			,ContactPhone = @contactPhone
			,ContactAddress = @contactAddress
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
            ,ModifiedBy = @modifiedBy
			,EducationBackground = @EducationBackground
        WHERE Id = @employeeId

		UPDATE dbo.EmployeeExt WITH (UPDLOCK, ROWLOCK)
        SET DomainUsername = @domainUsername
            ,Email = @email
			,IsSpecialUser = @isSpecialUser
			,Enabled = @enabled
            ,GroupId = @mainGroupID
            ,Roles = @roles
            ,JoinedDate = @joinedDate
            ,ConfirmedDate = @confirmedDate
            ,ResignedDate = @resignedDate
        WHERE Id = @employeeId

        DELETE dbo.EmployeeGroup_Employee
        WHERE empid = @employeeId

        INSERT INTO dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        VALUES (
            @mainGroupID
            ,@employeeId
            ,1
            )

        INSERT INTO dbo.EmployeeGroup_Employee (
            GroupId
            ,EmpID
            ,isMainGroup
            )
        SELECT val
            ,@employeeId
            ,0
        FROM dbo.fnc_strSplit(@subGroups, ',')

		INSERT INTO dbo.[Log](LogActionID, LogDate, Description, TrackingValue, UserID)
		VALUES(5, dbo.fnc_GetClientDateTime(), 'Update Employee', @logData, @employeeId)

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update Employee unsuccessfully.'
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_GetAllReminderLateInEarlyOut]
AS
/*
Created: 20230529@Brian.Nguyen
Task: Get all Employees must be created leave half day request.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	DECLARE @FromDate DATE
		,@ToDate DATETIME
	SET @ToDate = dbo.fnc_GetClientDateTime()
	SET @FromDate = DATEADD(DD, -1, @ToDate)

    SELECT att.AttendanceDate
		,att.EmpId
		,e.NickName
		,e.FullName
		,ec.ConversationId
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.Id
	LEFT JOIN DBO.EmployeeConversation as ec WITH (NOLOCK) ON e.Id = ec.EmpId
	WHERE att.AttendanceDate BETWEEN @FromDate AND @ToDate
	AND (ISNULL(att.LateIn, 0) > 60 OR ISNULL(att.EarlyOut, 0) > 60)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_GetByType] @fromtime DATETIME
    ,@totime DATETIME
    ,@type SMALLINT
    ,@empid INT = NULL
    ,@groupid INT = NULL
    ,@departmentid INT = NULL
    ,@excludeUserDelete BIT = 1
    ,@PageIndex INT = NULL
    ,@ItemsPerPage INT = NULL
    ,@TotalItem INT = NULL OUT
AS
/*
Created: 2012/03/17@Dan
Task: get attentance list of employees by type
DB: NexHR
Original: 

Revisions:    
    - 20250515@Henry.Dinh: Add DepartmentName
Param's Explanation:
    @type: 0-all, 1-missing checkin; 2-missing check out, 3: latein; 4-earlyout, 5- not work
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @note VARCHAR(200)
        ,@today DATETIME
        ,@now DATETIME

    SET @now = dateadd(hh, 7, GETUTCDATE())
    SET @today = convert(VARCHAR(10), @now, 111) -- use UTC because the server use GMT-4

    IF OBJECT_ID('tempdb..#EmployeeAttendance') IS NOT NULL
    BEGIN
        DROP TABLE #EmployeeAttendance
    END

    CREATE TABLE #EmployeeAttendance (
        AttendanceId INT
        ,Code VARCHAR(20)
        ,EmpId INT
        ,FullName NVARCHAR(50)
        ,NickName NVARCHAR(20)
        ,DepartmentName NVARCHAR(100)
        ,Team NVARCHAR(50)
        ,TeamCode VARCHAR(20)
        ,[Rank] INT
        ,Roles INT
        ,AttendanceDate DATETIME
        ,ShiftIf INT
        ,ShiftCode VARCHAR(10)
        ,InFixed DATETIME
        ,OutFixed DATETIME
        ,LateIn INT
        ,EarlyOut INT
        ,TotalWorkHours DECIMAL(18, 2)
        ,Absence BIT
        ,LeaveTypeId INT
        ,LeaveDay DECIMAL(18, 1)
        ,IsLateIn INT
        ,IsEarlyOut INT
        ,IsMissingCheckin INT
        ,IsMissingCheckOut INT
        ,Notes NVARCHAR(100)
		,IsWorkFromHome BIT
        )

    INSERT INTO #EmployeeAttendance
    SELECT T.Id AS AttendanceId 
        ,T.Code
        ,T.EmpId
        ,T.FullName
        ,T.NickName
        ,T.DepartmentName
        ,T.Team
        ,T.TeamCode
        ,T.[Rank]
        ,T.Roles
        ,T.AttendanceDate
        ,T.ShiftId
        ,T.ShiftCode
        ,T.InFixed
        ,T.OutFixed
        ,T.LateIn
        ,T.EarlyOut
        ,T.TotalWorkHours
        ,T.Absence
        ,T.LeaveTypeId
        ,T.LeaveDay
        ,T.isLateIn
        ,T.isEarlyOut
        ,T.isMissingCheckin
        ,T.isMissingCheckOut
        ,stuff(CASE 
                WHEN ISNULL(T.Notes, '') = '' THEN ''
                ELSE ', ' + T.Notes
                END + CASE 
                WHEN isLateIn = 1 THEN (', Late In')
                ELSE ''
                END + CASE 
                WHEN isEarlyOut = 1 THEN ', Early Out'
                ELSE ''
                END + CASE 
                WHEN isMissingCheckin = 1 THEN ', Missing Check-in'
                ELSE ''
                END + CASE 
                WHEN isMissingCheckOut = 1 THEN ', Missing Check-out'
                ELSE ''
                END + CASE 
                WHEN T.isHoliday = 1 THEN ', Public Holiday'
                ELSE ''
                END + CASE 
                WHEN isOffday = 1 THEN ', Off day'
                ELSE ''
                END + CASE 
                WHEN IsWorkFromHome = 1 THEN ', WFH'
                ELSE ''
                END + CASE 
                WHEN (
                        isNotWork = 1
                        AND T.isHoliday <> 1
                        AND T.isOffday <> 1
                        ) THEN ', Absent'
                ELSE CASE 
                        WHEN lv.NAME IS NULL THEN ''
                        ELSE (CASE 
                                WHEN (
                                        T.isHoliday <> 1
                                        AND T.isOffday <> 1
                                        ) THEN (', ' + lv.NAME)
                                ELSE '' END
                                ) + CASE 
                                WHEN LeaveDay = 0.5 THEN ' (Half day)'
                                ELSE ''
                                END
                        END
                END, 1, 2, '') AS Notes,
				IsWorkFromHome
    FROM (
        SELECT a.Id
            ,e.Code
            ,a.EmpId
            ,FullName
            ,NickName
            ,d.Name AS DepartmentName
            ,g.NAME [Team]
            ,g.Code [TeamCode]
            ,g.[Rank]
            ,e.Roles
            ,AttendanceDate
            ,a.ShiftId
            ,s.code AS ShiftCode
            ,a.InFixed
            ,a.OutFixed
            ,a.LateIn
            ,a.EarlyOut
            ,TotalWorkHours
            ,Absence
            ,a.LeaveTypeId
            ,a.LeaveDay
            ,a.Notes
			,c.IsWorkFromHome
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND ISNULL(Absence, 0) = 0
                    AND (
                        (
                            DATEDIFF(D, a.AttendanceDate, '2015-08-01') > 0
                            AND ISNULL(a.LateIn, 0) > 15
                            )
                        OR (
                            DATEDIFF(D, '2015-08-01', a.AttendanceDate) >= 0
                            AND ISNULL(a.LateIn, 0) > 0
                            )
                        ) THEN 1
                ELSE 0
                END isLateIn
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND isnull(Absence, 0) = 0
                    AND (
                        (
                            DATEDIFF(D, a.AttendanceDate, '2015-08-01') > 0
                            AND ISNULL(a.EarlyOut, 0) > 15
                            )
                        OR (
                            DATEDIFF(D, '2015-08-01', a.AttendanceDate) >= 0
                            AND ISNULL(a.EarlyOut, 0) > 0
                            )
                        ) THEN 1
                ELSE 0
                END isEarlyOut
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND isnull(Absence, 0) = 0
                    AND InFixed IS NULL THEN 1
                ELSE 0
                END isMissingCheckin
            ,CASE 
                WHEN e.Id NOT IN (88, 89, 123, 134, 153)
                    AND isnull(Absence, 0) = 0
                    AND OutFixed IS NULL
                    AND AttendanceDate < @today THEN 1
                ELSE 0
                END isMissingCheckOut
            ,CASE 
                WHEN (
                        e.Id NOT IN (88, 89, 123, 134, 153)
                        AND isnull(Absence, 0) = 1
                        AND c.IsOffDay = 0
                        AND c.IsHoliday = 0
                        AND ISNULL(a.LeaveDay, 0) <> 1
                        AND @now > (AttendanceDate + ' ' + s.EndCheckInTime)
                        ) THEN 1
                ELSE 0
                END isNotWork
            ,CASE 
                WHEN isnull(Absence, 0) = 1
                    AND c.IsOffDay = 1 THEN 1
                ELSE 0
                END isOffday
            ,CASE 
                WHEN isnull(Absence, 0) = 1
                    AND c.IsHoliday = 1 THEN 1
                ELSE 0
                END isHoliday
        FROM dbo.EmployeeAttendance a WITH (NOLOCK)
            LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.EmpId = a.EmpId
            AND c.Workdate = a.AttendanceDate
            INNER JOIN Employee e WITH (NOLOCK) ON a.Empid = e.Id
            LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.Id = a.ShiftId
            LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid = g.Id
            LEFT JOIN dbo.Department d WITH (NOLOCK) ON g.DepartmentId = d.DepartmentId
        WHERE e.IsSpecialUser = 0
			AND attendancedate BETWEEN @fromtime
                AND @totime
            AND (
                (
                    e.isDeleted IS NULL
                    OR e.isDeleted = 0
                    )
                OR @excludeUserDelete = 0
                )
            AND (
                @empid IS NULL
                OR a.empid = @empid
                )
            AND (
                @groupid IS NULL
                OR g.ID = @groupid
                )
            AND (
                @departmentid IS NULL
                OR g.DepartmentId = @departmentid
                )
        ) T
        LEFT JOIN LeaveType lv WITH (NOLOCK) ON lv.ID = T.LeaveTypeId
    WHERE @type = 0
        OR (
            @type = 1
            AND isMissingCheckin = 1
            )
        OR (
            @type = 2
            AND isMissingCheckout = 1
            )
        OR (
            @type = 3
            AND isLateIn = 1
            )
        OR (
            @type = 4
            AND isEarlyOut = 1
            )
        OR (
            @type = 5
            AND isNotWork = 1
            )

    IF @PageIndex IS NOT NULL
        AND @ItemsPerPage IS NOT NULL
    BEGIN
        SELECT *
        FROM #EmployeeAttendance
        ORDER BY AttendanceDate DESC
            ,[Rank]
            ,TeamCode
            ,Roles DESC
            ,Code OFFSET(@PageIndex * @ItemsPerPage) ROWS

        FETCH NEXT @itemsPerPage ROWS ONLY
    END
    ELSE
    BEGIN
        SELECT *
        FROM #EmployeeAttendance
        ORDER BY AttendanceDate DESC
            ,[Rank]
            ,TeamCode
            ,Roles DESC
            ,Code
    END

    SELECT wk.Id
        ,wk.FromDate
        ,(CASE 
            WHEN wk.RequestType IN (1, 2) THEN wk.FromDate
            ELSE wk.ToDate END
            ) AS ToDate
        ,wk.TotalDays
        ,CASE 
            WHEN wk.RequestType = 3 THEN wk.LateInMins
            ELSE wk.EarlyOutMins
            END AS TotalMins
        ,wk.CreatedDate
        -- ,wk.[Status]
        ,wk.Remark
        ,wk.RequestType AS RequestTypeId
        ,ISNULL((CASE 
            WHEN wk.RequestType = 5 THEN s1.Id
            ELSE s3.Id END
            ), 0) AS FromShiftId
        ,ISNULL((CASE 
            WHEN wk.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ), '') AS FromShiftName
        ,s2.Id AS ToShiftId
        ,s2.Code AS ToShiftName
        ,wk.EmpId AS RequesterId
        ,e.NickName
        ,e.FullName
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON wk.EmpId = e.Id
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON cl.empid = wk.empid
        AND DATEDIFF(D, cl.WorkDate, wk.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = wk.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = wk.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = cl.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = wk.LeaveTypeID
    WHERE wk.[Status] IN (1, 2)
        AND (
            (
                wk.ToDate IS NULL
                AND DATEDIFF(D, @fromtime, wk.FromDate) >= 0
                AND DATEDIFF(D, wk.FromDate, @totime) >= 0
                )
            OR (
                NOT (
                    DATEDIFF(D, @totime, wk.FromDate) > 0
                    OR DATEDIFF(D, wk.ToDate, @fromtime) > 0
                    )
                )
            )
        AND (
            (
                e.isDeleted IS NULL
                OR e.isDeleted = 0
                )
            OR @excludeUserDelete = 0
            )
        AND (
            @empid IS NULL
            OR wk.empid = @empid
            )
        AND (
            @groupid IS NULL
            OR e.GroupId = @groupid
            )

    SELECT @TotalItem = COUNT(*)
    FROM #EmployeeAttendance

    IF OBJECT_ID('tempdb..#EmployeeAttendance') IS NOT NULL
    BEGIN
        DROP TABLE #EmployeeAttendance
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_GetCheckInTimeToNotify] 
AS
/*
Created: 20240701@Oliver.Dang
Task: Get employee attendance to notify when checkin.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @Today DATETIME = dbo.fnc_GetClientDateTime()
	DECLARE @FromTime DATETIME = DATEADD(MINUTE, -5, @Today),
			@ToTime DATETIME = DATEADD(MINUTE, 5, @Today)
	
	--SELECT 363 as EmployeeId
	--	 ,'Oliver' as NickName
	--	 ,'a:1n1E_0q_MKn87JQjFcY4cUNN8zGV6bvS6Ro3r0hpxJajXRv7DkQBzWHTghLeJvPKBqYJxjzjx6LihOgdnKeIwPOg1WDfpOvdOnS5seek_idXMyy820POpN8W3EaPgvoIL' as ConversationId
	--	 ,'2024-04-24 8:42:00' as CheckinTime


    SELECT e.Id as EmployeeId
		  ,e.NickName
		  ,ec.ConversationId
		  ,ea.AttendanceDate
		  ,ea.InFixed as CheckInTime
    FROM dbo.EmployeeAttendance AS ea WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.ID = ea.EmpId
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK) ON e.Id = ec.EmpId
		LEFT JOIN dbo.LogReachedWorkingTime AS l WITH (NOLOCK) ON l.EmpId = ea.EmpId 
					AND ea.AttendanceDate = l.WorkDate 
					AND  l.InsertedType = 'NotifyCheckIn'
    WHERE ea.AttendanceDate = DATEADD(dd, 0, DATEDIFF(dd, 0, @Today))
		AND l.EmpId IS NULL
		AND ISNULL(ec.ConversationId, '') <> ''
        AND ea.InFixed IS NOT NULL
		AND ea.InFixed BETWEEN @FromTime AND @ToTime
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_GetCheckOutTimeToNotify] 
AS
/*
Created: 20240422@Oliver.Dang
Task: Get employee attendance to notify when checkout.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @Today DATETIME = dbo.fnc_GetClientDateTime()
	DECLARE @FromTime DATETIME = DATEADD(MINUTE, -5, @Today),
			@ToTime DATETIME = DATEADD(MINUTE, 5, @Today)
	
	--SELECT 363 as EmployeeId
	--	 ,'Ð?ng Hoàng H?i' as FullName
	--	 ,'Oliver' as NickName
	--	 ,'a:1n1E_0q_MKn87JQjFcY4cUNN8zGV6bvS6Ro3r0hpxJajXRv7DkQBzWHTghLeJvPKBqYJxjzjx6LihOgdnKeIwPOg1WDfpOvdOnS5seek_idXMyy820POpN8W3EaPgvoIL' as ConversationId
	--	 ,'2024-04-24 17:42:00' as CheckOutTime
	--	 , 8.25 as TotalWorkHours


    SELECT e.Id as EmployeeId
		  ,e.FullName
		  ,e.NickName
		  ,ec.ConversationId
		  ,ea.AttendanceDate
		  ,ea.OutFixed as CheckOutTime
		  ,ea.TotalWorkHours
    FROM dbo.EmployeeAttendance AS ea WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.ID = ea.EmpId
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK) ON e.Id = ec.EmpId
		LEFT JOIN dbo.LogReachedWorkingTime AS l WITH (NOLOCK) ON l.EmpId = ea.EmpId AND ea.AttendanceDate = l.WorkDate AND l.InsertedType = 'NotifyCheckOut'
    WHERE ea.AttendanceDate = DATEADD(dd, 0, DATEDIFF(dd, 0, @Today))
		AND l.EmpId IS NULL
		AND ISNULL(ec.ConversationId, '') <> ''
        AND ea.OutFixed IS NOT NULL
		AND ea.OutFixed BETWEEN @FromTime AND @ToTime
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_GetEmployeeReachedWorkingTime]
AS
/*
Created: 20190508@Quinn.Vo
Task: get employees which reached time out.
DB: NexHR
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATETIME = dbo.fnc_GetClientDateTime()
		,@i INT
		,@TempEmpId INT
		,@TotalWorkHoursToday DECIMAL(18, 2)
	
	-- test
	--declare @hour int = datediff(hh, dbo.fnc_GetClientDateTime(), '2019-05-16 17:00')
	--select dateadd(MI, @hour*60, @Today)

	CREATE TABLE #Employee (
		Id INT PRIMARY KEY IDENTITY(1, 1)
		,EmpId INT
		,TotalWorkHours DECIMAL(18, 2)
		)
	
	CREATE TABLE #TimeRecord (
		Id INT
		,EmpID INT
		,RecordTime DATETIME
		,InOutType TINYINT
		)

	CREATE TABLE #LeaveRequestHalfDay (
		EmpID INT
		,OnDate DATETIME
		,FromTime DATETIME
		,ToTime DATETIME
		)

	INSERT INTO #Employee (EmpId)
	SELECT EmpId
	FROM [dbo].[EmployeeAttendance] AS att
	WHERE DATEDIFF(MI, ExpectedOutTime, dateadd(MI, 0, @Today)) <= 5
		AND DATEDIFF(MI, ExpectedOutTime, dateadd(MI, 0, @Today)) >= 0
		AND OutFixed IS NULL
		AND InFixed IS NOT NULL
		AND ISNULL(LeaveDay, 0) <> 1
		AND NOT EXISTS (
			SELECT 1 FROM dbo.LogReachedWorkingTime WITH (NOLOCK)
			WHERE EmpId = att.EmpId
			AND WorkDate = att.AttendanceDate AND InsertedType = 'ReachedWorkingHours'
		)

	INSERT INTO #TimeRecord (
		Id
		,EmpID
		,RecordTime
		,InOutType
		)
	SELECT Id
		,EmpID
		,RecordTime
		,InOutType
	FROM dbo.TimeRecord AS ti WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM #Employee
			WHERE EmpId = ti.EmpID
			)
		AND DATEDIFF(d, @Today, RecordTime) >= 0
		AND DATEDIFF(d, @Today, RecordTime) <= 1

	---- insert virtual checkout time (now)
	INSERT INTO #TimeRecord (
		Id
		,EmpID
		,RecordTime
		,InOutType
		)
	SELECT NULL
		,EmpId
		,@Today
		,1
	FROM #Employee

	INSERT INTO #LeaveRequestHalfDay (
		EmpID
		,OnDate
		,FromTime
		,ToTime
		)
	SELECT EmpId
		,CONVERT(VARCHAR(10), FromDate, 111)
		,FromDate
		,ToDate
	FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
	WHERE [Status] = 2
		AND TotalDays = 0.5
		AND EXISTS (
			SELECT 1
			FROM #Employee
			WHERE EmpId = wkr.EmpID
			)
		AND DATEDIFF(d, dbo.fnc_GetClientDateTime(), FromDate) >= 0

	CREATE TABLE #EmpCalendar (
		Id INT PRIMARY KEY IDENTITY(1, 1)
		,EmpId INT
		,WorkDate DATETIME
		,ShiftId INT
		,IsHoliday BIT
		,IsOffDay BIT
		,IsLeaveDay BIT
		,LeaveTypeId INT NULL
		,LateIn INT
		,EarlyOut INT
		,RequestedCheckInTime DATETIME
		,RequestedCheckOutTime DATETIME
		,BeginCheckInTime DATETIME
		,EndCheckInTime DATETIME
		,BeginCheckOutTime DATETIME
		,EndCheckOutTime DATETIME
		,BreakTimeMins INT
		,BeginBreakTime DATETIME
		,EndBreakTime DATETIME
		,LeaveDay DECIMAL(18, 1)
		,TimeForPostnatal INT
		)

	INSERT INTO #EmpCalendar (
		EmpId
		,WorkDate
		,ShiftId
		,IsHoliday
		,IsOffDay
		,IsLeaveDay
		,LeaveTypeId
		,LateIn
		,EarlyOut
		,RequestedCheckInTime
		,RequestedCheckOutTime
		,BeginCheckInTime
		,EndCheckInTime
		,BeginCheckOutTime
		,EndCheckOutTime
		,BreakTimeMins
		,BeginBreakTime
		,EndBreakTime
		,LeaveDay
		,TimeForPostnatal
		)
	SELECT ec.EmpId
		,WorkDate
		,ec.ShiftId
		,IsHoliday
		,IsOffDay
		,IsLeaveDay
		,LeaveTypeId
		,LateIn
		,EarlyOut
		,RequestedCheckinTime
		,RequestedCheckoutTime
		,CASE 
			WHEN IsInAfterDay = 1 THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
			ELSE CAST(WorkDate + ' ' + s.BeginCheckInTime AS DATETIME)
			END AS [BeginCheckInTime]
		,CASE 
			WHEN IsInAfterDay = 1 THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
			ELSE CAST(WorkDate + ' ' + s.EndCheckInTime AS DATETIME)
			END AS [EndCheckInTime]
		,CASE 
			WHEN IsOutAfterDay = 1 THEN DATEADD(d, 1, WorkDate + ' ' + s.BeginCheckOutTime)
			ELSE CAST(WorkDate + ' ' + s.BeginCheckOutTime AS DATETIME)
			END AS [BeginCheckOutTime]
		,CASE 
			WHEN IsOutAfterDay = 1 THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckOutTime)
			ELSE CAST(WorkDate + ' ' + s.EndCheckOutTime AS DATETIME)
			END AS [EndCheckOutTime]
		,BreakTimeMins
		,CAST(WorkDate + ' ' + s.BeginBreakTime AS DATETIME)
		,CAST(WorkDate + ' ' + s.EndBreakTime AS DATETIME)
		,LeaveDay
		,CASE 
			WHEN pos.EmpId IS NULL THEN 0
			ELSE (CASE 
					WHEN (
							LeaveDay = 0.5
							AND LeaveTypeId <> 9
							) THEN (pos.[Minutes] / 2.0)
					ELSE pos.[Minutes] END
					)
			END
	FROM dbo.EmployeeCalendar ec WITH (NOLOCK)
		LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.Id = ec.ShiftId
		LEFT JOIN dbo.EmployeePostnatal pos WITH (NOLOCK) ON ec.EmpId = pos.EmpId
		AND ec.WorkDate BETWEEN pos.FromDate
			AND pos.ToDate
	WHERE EXISTS (
			SELECT 1
			FROM #Employee
			WHERE EmpId = ec.EmpID
			)
		AND ec.WorkDate = convert(DATE, @Today)
	ORDER BY ec.EmpId
		,ec.WorkDate

	SET @i = 1

	WHILE EXISTS (
			SELECT 1
			FROM #Employee
			WHERE Id = @i
			)
	BEGIN
		SET @TotalWorkHoursToday = 0

		SELECT @TempEmpId = EmpId
		FROM #Employee
		WHERE Id = @i

		EXEC dbo.EmployeeAttendance_TotalWorkingHoursToday_ForEmpId @empId = @TempEmpId
			,@TotalWorkHoursToday = @TotalWorkHoursToday OUTPUT

		UPDATE #Employee
		SET TotalWorkHours = @TotalWorkHoursToday
		WHERE Id = @i

		SET @i = @i + 1
	END

	SELECT e.Id AS EmployeeId
		,e.NickName
		,e.FullName
		,ee.TotalWorkHours
		,ec.ConversationId AS SkypeConversationId
		--,'29:19TyupFcn7CJOya4b7d76pGpzpmppzjOzppdk8JttHuU' AS SkypeConversationId
	FROM #Employee AS ee
	INNER JOIN dbo.Employee AS e ON ee.EmpId = e.Id
	INNER JOIN dbo.EmployeeConversation AS ec ON ee.EmpId = ec.EmpId

	--SELECT e.Id AS EmployeeId
	--	,e.NickName
	--	,e.FullName
	--	,8 AS TotalWorkHours
	--	--,ec.ConversationId AS SkypeConversationId
	--	,'a:18u9RpRBzAK8OdRfTfUWW0CKIVNv5X_FPrRCv6AYZBUmFTGZXbO8ZMkzSdBhxjKYsOPS0z9nZlGISQvyHy8vMwrcV0fO3NJmpII7d6yYrG4JN_IVcxuUuIJziYTeG2jPO' AS SkypeConversationId
	--FROM dbo.Employee AS e 
	--INNER JOIN dbo.EmployeeConversation AS ec ON e.Id = ec.EmpId
	--WHERE e.id = 125

	DROP TABLE #Employee
	DROP TABLE #EmpCalendar
	DROP TABLE #TimeRecord
	DROP TABLE #LeaveRequestHalfDay
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_GetMissingCheckInCheckOut]
AS
/*
Created: 20200706@Merlin.Le
Task: get missing checkin today and checkout yesterday to remind via Skype chanel
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
     CREATE TABLE #EmployeeAttendance
     (EmpId          INT,
      AttendanceDate DATETIME,
      MissingType    TINYINT
     );
     DECLARE @today DATETIME, @yesterday DATETIME, @now DATETIME, @startday DATETIME;
     SET @now = DATEADD(hh, 7, GETUTCDATE());
     SET @today = CONVERT(VARCHAR(10), @now, 111); --use UTC date because the server time is GMT -4, not GMT+7
     SET @yesterday = DATEADD(d, -1, @today);

     -- SELECT @startday = '2015-04-28', @yesterday = '2015-04-28'
     IF(DATEPART(WEEKDAY, @today) = 2
        OR DATEPART(WEEKDAY, @today) = 3
        OR DATEPART(WEEKDAY, @today) = 4)
         BEGIN
             SET @startday = DATEADD(d, -5, @today);
         END;
     ELSE
         BEGIN
             SET @startday = DATEADD(d, -3, @today);
         END;
             
        INSERT INTO #EmployeeAttendance
        (EmpId,
        AttendanceDate,
        MissingType
        )
            SELECT a.EmpId,
                    AttendanceDate,
                    1
            FROM [dbo].[EmployeeAttendance] AS a WITH (NOLOCK)
                    INNER JOIN [dbo].[EmployeeCalendar] AS c WITH (NOLOCK) ON c.empID = a.EmpID
                                                                    AND c.WorkDate = AttendanceDate
            WHERE attendancedate BETWEEN @startday AND @yesterday
                    --AND ISNULL(Absence, 0) = 0
                    AND InFixed IS NULL
                    AND isOffday = 0
                    AND IsHoliday = 0
                    AND (IsLeaveDay = 0
                        OR (IsLeaveDay = 1
                            AND a.LeaveDay = 0.5))
                    AND NOT EXISTS
            (
                SELECT 1
                FROM [dbo].[WorkingTimeRequest] AS r WITH (NOLOCK)
                WHERE r.empid = a.empid 
                        AND requesttype = 1
                        AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
            )
                    AND NOT EXISTS
            (
                SELECT 1
                FROM [dbo].[WorkingTimeRequest] AS r WITH (NOLOCK)
                WHERE r.empid = a.empid
                        AND requesttype = 6
                        AND r.TotalDays >= 1
                        AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
                        AND r.[Status] <> 3
            );
        INSERT INTO #EmployeeAttendance
        (EmpId,
        AttendanceDate,
        MissingType
        )
            SELECT a.EmpId,
                    AttendanceDate,
                    2
            FROM [dbo].[EmployeeAttendance] AS a WITH (NOLOCK)
                    INNER JOIN [dbo].[EmployeeCalendar] AS c WITH (NOLOCK) ON c.empID = a.EmpID
                                                                    AND c.WorkDate = AttendanceDate
            WHERE attendancedate BETWEEN @startday AND @yesterday
                    AND OutFixed IS NULL
                    AND isOffday = 0
                    AND IsHoliday = 0
                    AND (IsLeaveDay = 0
                        OR (IsLeaveDay = 1
                            AND a.LeaveDay = 0.5))
                    AND NOT EXISTS
            (
                SELECT 1
                FROM [dbo].[WorkingTimeRequest] AS r WITH (NOLOCK)
                WHERE r.empid = a.empid 
                        -- AND r.CreatedDate >= @yesterday 
                        AND requesttype = 2
                        AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
            )
                    AND NOT EXISTS
            (
                SELECT 1
                FROM [dbo].[WorkingTimeRequest] AS r WITH (NOLOCK)
                WHERE r.empid = a.empid
                        AND requesttype = 6
                        AND r.TotalDays >= 1
                        AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
                        AND r.[Status] <> 3
            );
        SELECT a.EmpId,
            FullName,
            e.NickName,
			ec.ConversationId,
            a.AttendanceDate,
            s.Code AS ShiftCode,
            a.MissingType AS [Type]
        FROM #EmployeeAttendance a
            INNER JOIN [dbo].[EmployeeCalendar] AS c WITH (NOLOCK) ON c.empID = a.EmpID
                                                                AND c.WorkDate = AttendanceDate
            INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON a.Empid = e.Id
			LEFT JOIN [dbo].[EmployeeConversation] AS ec WITH (NOLOCK) ON ec.EmpId = e.Id
            LEFT JOIN [dbo].[Shift] s WITH (NOLOCK) ON s.ID = c.ShiftId
		WHERE e.[Enabled] = 1 AND e.[isDeleted] = 0
		AND e.IsSpecialUser = 0
        ORDER BY a.AttendanceDate DESC,
                e.NickName;
     
     DROP TABLE #EmployeeAttendance;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_GetMissingCheckinOutToRemind] @EmpId INT = NULL
AS
/*
Created: 2012/03/17@Dan
Task: get missing checkin today and checkout yesterday to remind via email
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
     CREATE TABLE #EmployeeAttendance
     (EmpId          INT,
      AttendanceDate DATETIME,
      MissingType    TINYINT
     );
     DECLARE @today DATETIME, @yesterday DATETIME, @now DATETIME, @startday DATETIME;
     SET @now = DATEADD(hh, 7, GETUTCDATE());
     SET @today = CONVERT(VARCHAR(10), @now, 111); --use UTC date because the server time is GMT -4, not GMT+7
     SET @yesterday = DATEADD(d, -1, @today);

     -- SELECT @startday = '2015-04-28', @yesterday = '2015-04-28'
     IF(DATEPART(WEEKDAY, @today) = 2
        OR DATEPART(WEEKDAY, @today) = 3
        OR DATEPART(WEEKDAY, @today) = 4)
         BEGIN
             SET @startday = DATEADD(d, -5, @today);
         END;
     ELSE
         BEGIN
             SET @startday = DATEADD(d, -3, @today);
         END;
     IF @EmpId IS NULL
         BEGIN
             -- check to make sure checkin/out data has been impoted from Bioadmin
             IF(NOT EXISTS
               (
                   SELECT 1
                   FROM dbo.TimeRecord WITH (NOLOCK)
                   WHERE RecordTime > @yesterday+' 08:00'
               )
                OR DATEPART(WEEKDAY, @today) = 7
                OR DATEPART(WEEKDAY, @today) = 1) -- 1 : Sunday, 7 : Saturday
                 BEGIN
                     DROP TABLE #EmployeeAttendance;
                     RETURN;
                 END;
             INSERT INTO #EmployeeAttendance
             (EmpId,
              AttendanceDate,
              MissingType
             )
                    SELECT a.EmpId,
                           AttendanceDate,
                           1
                    FROM dbo.EmployeeAttendance a WITH (NOLOCK)
                         INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
                                                                            AND c.WorkDate = AttendanceDate
                    WHERE attendancedate BETWEEN @startday AND @yesterday
                          --AND ISNULL(Absence, 0) = 0
                          AND InFixed IS NULL
                          AND isOffday = 0
                          AND IsHoliday = 0
                          AND (IsLeaveDay = 0
                               OR (IsLeaveDay = 1
                                   AND a.LeaveDay = 0.5))
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid 
                              -- AND r.CreatedDate >= @yesterday 
                              AND requesttype = 1
                              AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
                    )
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid
                              AND requesttype = 6
                              AND r.TotalDays >= 1
                              AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
                              AND r.[Status] <> 3
                    );
             INSERT INTO #EmployeeAttendance
             (EmpId,
              AttendanceDate,
              MissingType
             )
                    SELECT a.EmpId,
                           AttendanceDate,
                           2
                    FROM dbo.EmployeeAttendance a WITH (NOLOCK)
                         INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
                                                                            AND c.WorkDate = AttendanceDate
                    WHERE attendancedate BETWEEN @startday AND @yesterday
                          --AND ISNULL(Absence, 0) = 0
                          AND OutFixed IS NULL
                          AND isOffday = 0
                          AND IsHoliday = 0
                          AND (IsLeaveDay = 0
                               OR (IsLeaveDay = 1
                                   AND a.LeaveDay = 0.5))
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid 
                              -- AND r.CreatedDate >= @yesterday 
                              AND requesttype = 2
                              AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
                    )
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid
                              AND requesttype = 6
                              AND r.TotalDays >= 1
                              AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
                              AND r.[Status] <> 3
                    );
             SELECT a.EmpId,
                    FullName,
                    NickName,
                    g.Name [Team],
                    g.Code [TeamCode],
                    AttendanceDate,
                    s.Code ShiftCode,
                    Email,
                    a.MissingType AS [Type]
             FROM #EmployeeAttendance a
                  INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empID = a.EmpID
                                                                     AND c.WorkDate = AttendanceDate
                  INNER JOIN Employee e WITH (NOLOCK) ON a.Empid = e.Id
                  LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid = g.Id
                  LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ID = c.ShiftId
             ORDER BY a.AttendanceDate DESC,
                      e.NickName;
         END;
     ELSE
     /*
     This case was added by Harvey on 03/02/2016
     - Reason: get missing check in/out by EmpId to remind
     - Task: Todo list - Dashboard
     */
         BEGIN
             INSERT INTO #EmployeeAttendance
             (EmpId,
              AttendanceDate,
              MissingType
             )
                    SELECT a.EmpId,
                           AttendanceDate,
                           1
                    FROM dbo.EmployeeAttendance a WITH (NOLOCK)
                         INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
                                                                            AND c.WorkDate = AttendanceDate
						INNER JOIN dbo.Shift AS sh ON c.ShiftId = sh.Id
                    WHERE attendancedate BETWEEN @startday AND @today
                          --AND ISNULL(Absence, 0) = 0
                          AND InFixed IS NULL
						  AND dbo.fnc_GetClientDateTime() > c.WorkDate + ' ' + sh.EndCheckInTime
                          AND a.EmpId = @EmpId
                          AND isOffday = 0
                          AND IsHoliday = 0
                          AND (IsLeaveDay = 0
                               OR (IsLeaveDay = 1
                                   AND a.LeaveDay = 0.5))
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid 
                              -- AND r.CreatedDate >= @yesterday 
                              AND a.EmpId = @EmpId
                              AND requesttype = 1
                              AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
                    )
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid
                              AND requesttype = 6
                              AND a.EmpId = @EmpId
                              AND r.TotalDays >= 1
                              AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
                              AND r.[Status] <> 3
                    );
             INSERT INTO #EmployeeAttendance
             (EmpId,
              AttendanceDate,
              MissingType
             )
                    SELECT a.EmpId,
                           AttendanceDate,
                           2
                    FROM dbo.EmployeeAttendance a WITH (NOLOCK)
                         INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
                                                                            AND c.WorkDate = AttendanceDate
						INNER JOIN dbo.Shift AS sh ON c.ShiftId = sh.Id
                    WHERE attendancedate BETWEEN @startday AND @yesterday
                          AND a.EmpId = @EmpId
						  AND dbo.fnc_GetClientDateTime() > c.WorkDate + ' ' + sh.EndCheckOutTime
                          --AND ISNULL(Absence, 0) = 0
                          AND OutFixed IS NULL
                          AND isOffday = 0
                          AND IsHoliday = 0
                          AND (IsLeaveDay = 0
                               OR (IsLeaveDay = 1
                                   AND a.LeaveDay = 0.5))
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid 
                              -- AND r.CreatedDate >= @yesterday 
                              AND requesttype = 2
                              AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
                    )
                          AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
                        WHERE r.empid = a.empid
                              AND requesttype = 6
                              AND r.TotalDays >= 1
                              AND AttendanceDate BETWEEN r.FromDate AND r.ToDate
                              AND r.[Status] <> 3
                    );
             SELECT a.EmpId,
                    FullName,
                    NickName,
                    g.Name [Team],
                    g.Code [TeamCode],
                    AttendanceDate,
                    s.Code ShiftCode,
                    Email,
                    a.MissingType
             FROM #EmployeeAttendance a
                  INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empID = a.EmpID
                                                                     AND c.WorkDate = AttendanceDate
                  INNER JOIN Employee e WITH (NOLOCK) ON a.Empid = e.Id
                  LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid = g.Id
                  LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ID = c.ShiftId
             ORDER BY a.AttendanceDate DESC,
                      e.NickName;
         END;
     DROP TABLE #EmployeeAttendance
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert] --null, '2015-10-12', '2015-10-12' 
    @empid INT,
    @fromTime DATETIME, 
    @toTime DATETIME,
    @onlyUpdate BIT = 1
AS
BEGIN
DECLARE @i INT, @TempEmpId INT, @CurrentDateTime DATETIME, @TempResignedDate DATETIME

SELECT @fromTime = CAST(@fromTime AS DATE), @toTime = CAST(@toTime AS DATE), @CurrentDateTime = dbo.fnc_GetClientDateTime()

IF(@onlyUpdate = 0 
AND (
(DATEPART(HOUR, @CurrentDateTime) >= 18 AND DATEPART(HOUR, @CurrentDateTime) < 24) 
OR (DATEPART(HOUR, @CurrentDateTime) >= 10 AND DATEPART(HOUR, @CurrentDateTime) < 11)
))
BEGIN
    SET @onlyUpdate = 1
END

IF OBJECT_ID('#Employee') IS NOT NULL
BEGIN
    DROP TABLE #Employee
END 
CREATE TABLE #Employee (Id INT PRIMARY KEY IDENTITY(1,1), EmpId INT, ResignedDate DATETIME)

IF OBJECT_ID('#TimeRecord') IS NOT NULL
BEGIN
    DROP TABLE #TimeRecord
END 
CREATE TABLE #TimeRecord (Id INT, EmpID INT, RecordTime DATETIME, InOutType TINYINT NULL)

-- Get all in/out data into temp table to calculate
INSERT INTO #TimeRecord    (Id, EmpID, RecordTime, InOutType)
SELECT tr.Id, EmpID, RecordTime, InOutType 
FROM dbo.TimeRecord AS tr WITH (NOLOCK)
LEFT JOIN dbo.TimeRecordExt AS te WITH (NOLOCK) ON tr.Id = te.ID
WHERE (@empId IS NULL OR EmpID = @empId)
    AND DATEDIFF(d, @fromTime, RecordTime) >= 0
    AND DATEDIFF(d, @toTime, RecordTime) <= 1
	AND te.ID IS NULL OR (te.[Status] = 1 OR te.[Status] = 2 OR te.[Status] = 4)

IF OBJECT_ID('#LeaveRequestHalfDay') IS NOT NULL
BEGIN
    DROP TABLE #LeaveRequestHalfDay
END 
CREATE TABLE #LeaveRequestHalfDay (EmpID INT, OnDate DATETIME, FromTime DATETIME, ToTime DATETIME)

-- Get all leave request half day
INSERT INTO #LeaveRequestHalfDay (EmpID, OnDate, FromTime, ToTime)
SELECT EmpId, CONVERT(VARCHAR(10), FromDate, 111), FromDate, ToDate
FROM dbo.WorkingTimeRequest WITH (NOLOCK)
WHERE [Status] IN (1, 2)
    AND TotalDays = 0.5
    AND (@empId IS NULL OR EmpId = @empId)
    AND DATEDIFF(d, @fromTime, FromDate) >= 0
    AND DATEDIFF(d, FromDate, @toTime) >= 0

-- Get all calendars of employees into temp table
IF OBJECT_ID('#EmpCalendar') IS NOT NULL
BEGIN
    DROP TABLE #EmpCalendar
END 
CREATE TABLE #EmpCalendar (
        Id INT PRIMARY KEY IDENTITY(1,1), 
        EmpId INT,
        WorkDate DATETIME ,
        ShiftId INT,
        IsHoliday BIT,
        IsOffDay BIT,
        IsLeaveDay BIT,
        LeaveTypeId INT NULL,
		LeaveTypeId2 INT NULL,
        LateIn INT,
        EarlyOut INT,
        RequestedCheckInTime DATETIME ,
        RequestedCheckOutTime DATETIME ,
        BeginCheckInTime DATETIME,
        EndCheckInTime DATETIME,
        BeginCheckOutTime DATETIME,
        EndCheckOutTime DATETIME,
        BreakTimeMins INT,
        BeginBreakTime DATETIME,
        EndBreakTime DATETIME,
        LeaveDay DECIMAL(18,1),
        TimeForPostnatal INT)

INSERT INTO #EmpCalendar (
        EmpId,
        WorkDate,
        ShiftId,
        IsHoliday,
        IsOffDay,
        IsLeaveDay,
        LeaveTypeId,
		LeaveTypeId2,
        LateIn,
        EarlyOut,        
        RequestedCheckInTime,
        RequestedCheckOutTime,
        BeginCheckInTime,
        EndCheckInTime,
        BeginCheckOutTime,
        EndCheckOutTime,
        BreakTimeMins,
        BeginBreakTime,
        EndBreakTime,
        LeaveDay,
        TimeForPostnatal)
SELECT     
        ec.EmpId,
        WorkDate,
        ec.ShiftId,
        IsHoliday,
        IsOffDay,
        IsLeaveDay,
        LeaveTypeId,
		LeaveTypeId2,
        LateIn,
        EarlyOut,        
        RequestedCheckinTime,
        RequestedCheckoutTime,
        CASE WHEN IsInAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
            ELSE CAST(WorkDate + ' ' + s.BeginCheckInTime AS DATETIME)
            END AS [BeginCheckInTime],
        CASE WHEN IsInAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
            ELSE CAST(WorkDate + ' ' + s.EndCheckInTime AS DATETIME)
            END AS [EndCheckInTime],

        CASE WHEN IsOutAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.BeginCheckOutTime )
            ELSE CAST(WorkDate + ' ' + s.BeginCheckOutTime AS DATETIME)
            END AS [BeginCheckOutTime],

        CASE WHEN IsOutAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckOutTime )
            ELSE CAST(WorkDate + ' ' + s.EndCheckOutTime AS DATETIME)
            END AS [EndCheckOutTime],
        BreakTimeMins,
        CAST(WorkDate + ' ' + s.BeginBreakTime AS DATETIME),
        CAST(WorkDate + ' ' + s.EndBreakTime AS DATETIME),
        LeaveDay,
        CASE WHEN pos.EmpId IS NULL THEN 0 ELSE (CASE WHEN (LeaveDay = 0.5 AND LeaveTypeId <> 9) THEN 30 ELSE pos.[Minutes] END) END
FROM dbo.EmployeeCalendar ec WITH (NOLOCK)
LEFT JOIN dbo.Shift s WITH (NOLOCK)ON s.Id = ec.ShiftId
LEFT JOIN dbo.EmployeePostnatal pos WITH (NOLOCK) ON ec.EmpId = pos.EmpId
AND ec.WorkDate BETWEEN pos.FromDate AND pos.ToDate
WHERE (@EmpId IS NULL OR ec.EmpId = @EmpId)
AND ec.WorkDate BETWEEN @FromTime AND @ToTime
ORDER BY ec.EmpId, ec.WorkDate

IF @empId IS NULL
BEGIN
    -- Get all enable staff to calculate 
    INSERT INTO #Employee (EmpId, ResignedDate)
    SELECT Id, ResignedDate FROM dbo.Employee WITH (NOLOCK) 
	WHERE [Enabled] = 1 
	AND ISNULL(isDeleted, 0) = 0
	AND IsSpecialUser = 0
END
ELSE
BEGIN
    INSERT INTO #Employee (EmpId, ResignedDate)
    SELECT Id, ResignedDate FROM dbo.Employee WITH (NOLOCK) WHERE Id = @empid
END

SET @i = 1
WHILE EXISTS (SELECT 1 FROM #Employee WHERE Id = @i)
BEGIN
    SELECT @TempEmpId = EmpId, @TempResignedDate = ResignedDate FROM #Employee WHERE Id = @i
    IF (DATEDIFF(D, @fromTime, '2015-08-01') > 0 AND DATEDIFF(D, '2015-08-01', @toTime) >= 0)
    BEGIN
        EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId @TempEmpId, @fromTime, '2015-07-31', @onlyUpdate
        EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId_new @TempEmpId, '2015-08-01', @toTime, @onlyUpdate
    END
    ELSE IF (DATEDIFF(D, @fromTime, '2015-08-01') > 0)
    BEGIN
        EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId @TempEmpId, @fromTime, @toTime, @onlyUpdate
    END
    ELSE
    BEGIN
        IF(@TempResignedDate IS NOT NULL OR DATEDIFF(D, @TempResignedDate, @toTime) > 0)
        BEGIN
            DELETE FROM EmployeeAttendance WHERE EmpId = @TempEmpId AND AttendanceDate > @TempResignedDate
            EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId_new @TempEmpId, @fromTime, @TempResignedDate, @onlyUpdate
        END
        ELSE
        BEGIN
            EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId_new @TempEmpId, @fromTime, @toTime, @onlyUpdate
        END
    END
    SET @i = @i + 1
END

DROP TABLE #TimeRecord
DROP TABLE #EmpCalendar
DROP TABLE #LeaveRequestHalfDay
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForEmpId]
    @empId INT, 
    @fromTime DATETIME , 
    @toTime DATETIME ,
    @onlyUpdate BIT = 0
AS
BEGIN
    DECLARE @RangeToScanTime INT
    DECLARE @DeductEarlyOut INT, @BreakTime INT, @tempTimeIn DATETIME
    SET @RangeToScanTime = 480 -- 480 munites

    DECLARE @TimeRecord TABLE (Id INT, EmpID INT, RecordTime DATETIME, InOutType TINYINT)
    INSERT INTO @TimeRecord    (Id, EmpID, RecordTime, InOutType)
    SELECT Id, EmpID, RecordTime, InOutType 
    FROM #TimeRecord
    WHERE EmpID = @empId
        AND DATEDIFF(d, @fromTime, RecordTime) >= 0
        AND DATEDIFF(d, @toTime, RecordTime) <= 1

    DELETE FROM #TimeRecord
    WHERE EmpID = @empId
        AND DATEDIFF(d, @fromTime, RecordTime) >= 0
        AND DATEDIFF(d, @toTime, RecordTime) <= 1

    DECLARE @i INT, @attendaceDate DATETIME
    DECLARE    @ID INT, 
        @EmpIdCur INT,
        @WorkDate DATETIME,
        @ShiftId INT,
        @IsHoliday BIT,
        @isOffDay BIT,
        @IsLeaveDay BIT,
        @LeaveTypeId INT,
        @LateIn INT,
        @EarlyOut INT,
        @RequestedCheckinTime DATETIME,
        @RequestedCheckoutTime DATETIME,
        @BeginCheckInTime DATETIME,
        @EndCheckInTime DATETIME,
        @BeginCheckOutTime DATETIME,
        @EndCheckOutTime DATETIME,
        @InOfMachine DATETIME,
        @OutOfMachine DATETIME,
        @ExpectedInTime DATETIME,
        @ExpectedOutTime DATETIME,
        @Absence BIT,
        @IsRightShift BIT,
        @ActualLateIn INT,
        @ActualEarlyOut INT,
        @TotalWorkHours DECIMAL(18,2),
        @BreakTimeMins INT,
        @BeginBreakTime DATETIME,
        @EndBreakTime DATETIME,
        @LeaveDay DECIMAL(18,1),
        @Notes NVARCHAR(2000),
        @RequestLeaveTimeBegin DATETIME,
        @RequestLeaveTimeEnd DATETIME,
        @adjustLateIn INT,
        @adjustEarlyOut INT,
        @TimeForPostnatal INT
    
    SET @i = 0
    SET @attendaceDate = CONVERT(VARCHAR(10), @fromTime, 111)
    WHILE (DATEDIFF(d, @attendaceDate, @toTime) >= 0)
    BEGIN 
        
        SELECT @Absence = 0,  @InOfMachine = null, @OutOfMachine = null, 
            @ID = null, @ExpectedOutTime  = null, @IsRightShift=null,
            @ExpectedInTime = null,    @ExpectedOutTime = null, 
            @ActualLateIn = 0, @ActualEarlyOut = 0,
            @RequestedCheckinTime = null, @RequestedCheckoutTime = null,
            @TotalWorkHours = null, @BreakTimeMins = 0,
            @ShiftId = null, @LeaveTypeId = null,
            @RequestLeaveTimeBegin = null, @RequestLeaveTimeEnd = null,
            @adjustLateIn = 0, @adjustEarlyOut = 0, @DeductEarlyOut = 0,
            @BeginBreakTime = null, @EndBreakTime = null, @TimeForPostnatal = 0, @tempTimeIn = null

        SELECT 
            @WorkDate = WorkDate,
            @ShiftId = ShiftId,
            @IsHoliday = IsHoliday,
            @isOffDay = IsOffDay,
            @IsLeaveDay = IsLeaveDay,
            @LeaveTypeId = LeaveTypeId,
            @LateIn = ISNULL(LateIn,0),
            @EarlyOut = ISNULL(EarlyOut,0),
            @RequestedCheckinTime = RequestedCheckinTime,
            @RequestedCheckoutTime = RequestedCheckoutTime,
            @BeginCheckInTime = BeginCheckInTime,
            @EndCheckInTime = EndCheckInTime,
            @BeginCheckOutTime = BeginCheckOutTime,
            @EndCheckOutTime = EndCheckOutTime,
            @BreakTimeMins  = BreakTimeMins,
            @BeginBreakTime = BeginBreakTime,
            @EndBreakTime = EndBreakTime,
            @LeaveDay = LeaveDay,
            @TimeForPostnatal = TimeForPostnatal
        FROM #EmpCalendar
        WHERE EmpId = @empId AND WorkDate = @attendaceDate

        -- If Has Shift
        IF @ShiftId IS NOT NULL
        BEGIN
            IF(@ShiftId = 1) -- shift E3, E4, E5
            BEGIN
                SELECT @InOfMachine = MIN(RecordTime)
                FROM @TimeRecord 
                WHERE InOutType = 0
                    AND DATEDIFF(d, @attendaceDate, RecordTime) = 0

                IF @RequestedCheckinTime IS NOT NULL
                BEGIN
                    SET @InOfMachine = @RequestedCheckinTime
                END

                SELECT @OutOfMachine = MIN(RecordTime)
                FROM @TimeRecord 
                WHERE InOutType = 1
                    AND DATEDIFF(d, @attendaceDate, RecordTime) = 0
                    AND (@InOfMachine IS NULL OR RecordTime > DATEADD(MI, 2, @InOfMachine))

                IF @RequestedCheckoutTime IS NOT NULL
                BEGIN
                    SET @OutOfMachine = @RequestedCheckoutTime
                END

                IF (@InOfMachine IS NOT NULL)
                BEGIN
                    IF (@LateIn > 0)
                    BEGIN
                        SET @tempTimeIn = DATEADD(MI, -@LateIn, @InOfMachine)
                    END
                    ELSE
                    BEGIN
                        SET @tempTimeIn = @InOfMachine
                    END

                    IF(ISNULL(@LeaveDay, 0) = 0)
                    BEGIN
                        IF (DATEDIFF(MI, @tempTimeIn, @BeginCheckInTime) >= 0) -- <8h
                        BEGIN
                            SELECT @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime
                        END
                        ELSE IF (DATEDIFF(MI, @tempTimeIn, @EndCheckInTime) >= 0) -- 8->9h
                        BEGIN
                            SELECT @ExpectedInTime = @tempTimeIn, @ExpectedOutTime = DATEADD(HH, 9, @tempTimeIn)
                        END
                        ELSE IF (DATEDIFF(MI, @tempTimeIn, DATEADD(HH, 1, @EndCheckInTime)) >= 0) -- 9h->10h
                        BEGIN
                            SELECT @ExpectedInTime = @EndCheckInTime, @ExpectedOutTime = @EndCheckOutTime
                        END
                        ELSE -- >10h
                        BEGIN
                            SELECT @ExpectedInTime = @EndCheckInTime, @ExpectedOutTime = @EndCheckOutTime
                        END
                    END
                    ELSE IF (@LeaveDay = 0.5)
                    BEGIN
                        IF (@LeaveTypeId = 9) -- business trip
                        BEGIN
                            SELECT @RequestLeaveTimeBegin = FromTime, @RequestLeaveTimeEnd = ToTime
                            FROM #LeaveRequestHalfDay
                            WHERE EmpId = @empId AND OnDate = @attendaceDate
                            IF (DATEDIFF(MI, @RequestLeaveTimeBegin, @EndCheckInTime) >= 0)
                            BEGIN
                                IF (DATEDIFF(MI, @BeginBreakTime, @RequestLeaveTimeEnd) >= 0 
                                    AND DATEDIFF(MI, @RequestLeaveTimeEnd, @EndBreakTime) >= 0)
                                BEGIN
                                    IF (DATEDIFF(MI, @InOfMachine, DATEADD(MI, 90, @EndBreakTime)) >= 0)
                                    BEGIN
                                        SELECT @ExpectedInTime = @InOfMachine, @ExpectedOutTime = DATEADD(HH, 4, @InOfMachine)
                                    END
                                    ELSE
                                    BEGIN
                                        SELECT @ExpectedInTime = DATEADD(MI, 90, @EndBreakTime), @ExpectedOutTime = @EndCheckOutTime
                                    END
                                END
                                ELSE
                                BEGIN
                                    SELECT @ExpectedInTime = @RequestLeaveTimeEnd, @ExpectedOutTime = DATEADD(HH, 9, @RequestLeaveTimeBegin)
                                END
                            END
                            ELSE IF (DATEDIFF(MI, @BeginCheckOutTime, @RequestLeaveTimeEnd) >= 0)
                            BEGIN
                                SELECT @ExpectedInTime = DATEADD(HH, -9, @RequestLeaveTimeEnd), @ExpectedOutTime = @RequestLeaveTimeBegin
                            END
                            ELSE
                            BEGIN
                                SELECT @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime
                            END
                        END
                        ELSE
                            BEGIN
                            IF (DATEDIFF(MI, @tempTimeIn, @BeginCheckInTime) >= 0) -- <8h
                            BEGIN
                                SELECT @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = DATEADD(HH, 4, @BeginCheckInTime)
                            END
                            ELSE IF (DATEDIFF(MI, @tempTimeIn, @EndCheckInTime) >= 0) -- 8->9h
                            BEGIN
                                SELECT @ExpectedInTime = @tempTimeIn, @ExpectedOutTime = DATEADD(HH, 4, @tempTimeIn)
                            END
                            ELSE IF (DATEDIFF(MI, @tempTimeIn, DATEADD(HH, 1, @EndCheckInTime)) >= 0) -- 9h->10h
                            BEGIN
                                SELECT @RequestLeaveTimeBegin = FromTime, @RequestLeaveTimeEnd = ToTime
                                FROM #LeaveRequestHalfDay
                                WHERE EmpId = @empId AND OnDate = @attendaceDate

                                IF(@RequestLeaveTimeBegin < @EndBreakTime)
                                BEGIN
                                    SELECT @ExpectedInTime = @EndBreakTime, @ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                                END
                                ELSE
                                BEGIN
                                    SELECT @ExpectedInTime = @EndCheckInTime, @ExpectedOutTime = DATEADD(HH, 4, @EndCheckInTime)
                                END
                            END
                            ELSE IF (DATEDIFF(MI, @tempTimeIn, @EndBreakTime) >= 0) -- 10h->12h30
                            BEGIN
                                SELECT @ExpectedInTime = @EndBreakTime, @ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                            END
                            ELSE IF (DATEDIFF(MI, @tempTimeIn, DATEADD(HH, 5, @EndCheckInTime)) >= 0) -- 12h30-->14h00
                            BEGIN
                                SELECT @ExpectedInTime = @tempTimeIn, @ExpectedOutTime = DATEADD(HH, 4, @tempTimeIn)
                            END
                            ELSE -- >14h
                            BEGIN
                                SELECT @ExpectedInTime = DATEADD(HH, 5, @EndCheckInTime), @ExpectedOutTime = @EndCheckOutTime
                            END
                        END
                    END

                    IF (DATEADD(mi, @EarlyOut, @OutOfMachine) < @ExpectedOutTime)
                    BEGIN
                        -- SELECT @EarlyOut, @OutOfMachine, @ExpectedOutTime, @tempTimeIn
                        SET @ActualEarlyOut = DATEDIFF (MI, DATEADD(mi, @EarlyOut, @OutOfMachine), @ExpectedOutTime) - @DeductEarlyOut
                        IF (@ActualEarlyOut < 0)
                        BEGIN
                            SET @ActualEarlyOut = 0
                        END
                    END
                    IF (@tempTimeIn >  @ExpectedINTime) 
                    BEGIN
                        SET @ActualLateIn = DATEDIFF (MI, @ExpectedINTime, @tempTimeIn)
                        IF (@ActualLateIn < 0)
                        BEGIN
                            SET @ActualLateIn = 0
                        END
                    END
                END
                ELSE
                BEGIN
                    SELECT @ExpectedInTime = @EndCheckInTime, @ExpectedOutTime = @EndCheckOutTime
                END

                -- Break time for total hours
                IF(@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL)
                BEGIN
                    SET @BreakTime = 0
                    --IF (@InOfMachine < @BeginCheckInTime)
                    --BEGIN
                    --    SET @BreakTimeMins = DATEDIFF(MI, @InOfMachine, @BeginCheckInTime)
                    --END

                    IF (@InOfMachine <= @BeginBreakTime AND @OutOfMachine >= @EndBreakTime)
                    BEGIN
                        SET @BreakTime = @BreakTime + @BreakTimeMins
                    END
                    ELSE IF (@InOfMachine >= @BeginBreakTime AND @OutOfMachine <= @EndBreakTime)
                    BEGIN
                        SET @BreakTime = 1440
                    END
                    ELSE IF (@InOfMachine >= @BeginBreakTime
                        AND @InOfMachine <= @EndBreakTime
                        AND @OutOfMachine >= @EndBreakTime
                        )
                    BEGIN
                        SET @BreakTime = @BreakTime + (DATEDIFF(MI, @InOfMachine, @EndBreakTime))
                    END
                    ELSE IF (@InOfMachine <= @BeginBreakTime
                        AND @OutOfMachine >= @BeginBreakTime
                        AND @OutOfMachine <= @EndBreakTime
                        )
                    BEGIN
                        SET @BreakTime = @BreakTime + (DATEDIFF(MI, @BeginBreakTime, @OutOfMachine))
                    END

                    SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine) - @BreakTime)/ 60.0 
                    IF(@TotalWorkHours < 0)
                    BEGIN
                        SET @TotalWorkHours = 0
                    END
                    
                    IF (@LeaveDay = 0.5 AND @LeaveTypeId = 9 AND @TotalWorkHours >= 8)
                    BEGIN
                        SELECT @ActualLateIn = 0, @ActualEarlyOut = 0
                    END

                END
                --SELECT @ExpectedInTime, @ExpectedOutTime, @InOfMachine, @OutOfMachine, @BreakTime, @BreakTimeMins, @BeginBreakTime, @EndBreakTime
            END
            ELSE -- shift # E3, E4, E5
            BEGIN

                SET @ExpectedInTime = @BeginCheckInTime
                SET @ExpectedOutTime = @BeginCheckOutTime
                -- get time in
                SELECT @InOfMachine = MIN(RecordTime)
                FROM @TimeRecord 
                WHERE InOutType = 0
                    AND RecordTime BETWEEN DATEADD(mi, -@RangeToScanTime, @ExpectedInTime) AND @ExpectedOutTime

                -- get time out
                SELECT @OutOfMachine = MIN(RecordTime)
                FROM @TimeRecord 
                WHERE InOutType = 1
                    AND RecordTime BETWEEN @ExpectedInTime AND DATEADD(mi, @RangeToScanTime, @ExpectedOutTime)
                    AND (@InOfMachine IS NULL OR RecordTime > @InOfMachine)
        
                IF @InOfMachine > @OutOfMachine
                BEGIN
                    SET @OutOfMachine = NULL
                END

                IF @RequestedCheckinTime IS NOT NULL
                BEGIN
                    SET @InOfMachine = @RequestedCheckinTime
                END

                IF @RequestedCheckoutTime IS NOT NULL
                BEGIN
                    SET @OutOfMachine = @RequestedCheckoutTime
                END

                IF (DATEADD(mi, @EarlyOut, @OutOfMachine) < @ExpectedOutTime)
                BEGIN
                    SET @ActualEarlyOut = DATEDIFF (MINUTE, DATEADD(mi, @EarlyOut, @OutOfMachine), @ExpectedOutTime)    
                END
                IF (DATEADD(mi, -@LateIn, @InOfMachine) >  @ExpectedINTime) 
                BEGIN
                    SET @ActualLateIn = DATEDIFF (MINUTE, @ExpectedINTime, DATEADD(mi,-@LateIn , @InOfMachine)) 
                END
            
                IF (@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL)
                BEGIN
                    SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine) - ISNULL(@BreakTimeMins, 0))/ 60.0 

                    IF(@IsLeaveDay = 1 and @LeaveDay = 0.5)
                    BEGIN
                        
                        SELECT @RequestLeaveTimeBegin = FromTime, @RequestLeaveTimeEnd = ToTime
                        FROM #LeaveRequestHalfDay
                        WHERE EmpId = @empId AND OnDate = @attendaceDate
                        IF(@RequestLeaveTimeBegin IS NOT NULL AND @RequestLeaveTimeEnd IS NOT NULL)
                        BEGIN
                            IF(@TotalWorkHours >= 4)
                            BEGIN
                                SELECT @ActualLateIn = 0, @ActualEarlyOut = 0
                            END
                            ELSE
                                BEGIN
                                IF(DATEDIFF(mi, @ExpectedInTime, @RequestLeaveTimeBegin) < 5 AND DATEDIFF(mi, @ExpectedInTime, @RequestLeaveTimeBegin) > -5)
                                BEGIN
                                    SET @adjustLateIn = DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)
                                    SET @ActualLateIn = @ActualLateIn - @adjustLateIn
                                    IF @ActualLateIn < 0
                                    BEGIN
                                        SET @ActualLateIn = 0
                                    END
                                END
                                ELSE IF (DATEDIFF(mi, @ExpectedOutTime, @RequestLeaveTimeEnd) < 5 AND DATEDIFF(mi, @ExpectedOutTime, @RequestLeaveTimeEnd) > -5)
                                BEGIN
                                    SET @adjustEarlyOut = DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)
                                    SET @ActualEarlyOut = @ActualEarlyOut - @adjustEarlyOut
                                    IF @ActualEarlyOut < 0
                                    BEGIN
                                        SET @ActualEarlyOut = 0
                                    END
                                END
                            END
                        END
                    END
                    -- end xu ly nghi 1/2 ngay
                END
            END

            -- hau san 7h
            IF(@TimeForPostnatal > 0 AND @ActualLateIn > 0)
            BEGIN
                IF (@TimeForPostnatal > @ActualLateIn)
                BEGIN
                    SET @TimeForPostnatal = @TimeForPostnatal - @ActualLateIn
                    SET @ActualLateIn = 0
                END
                ELSE
                BEGIN
                    SET @ActualLateIn = @ActualLateIn - @TimeForPostnatal
                    SET @TimeForPostnatal = 0
                END
            END

            IF(@TimeForPostnatal > 0 AND @ActualEarlyOut > 0)
            BEGIN
                IF (@TimeForPostnatal > @ActualEarlyOut)
                BEGIN
                    SET @TimeForPostnatal = @TimeForPostnatal - @ActualEarlyOut
                    SET @ActualEarlyOut = 0
                END
                ELSE
                BEGIN
                    SET @ActualEarlyOut = @ActualEarlyOut - @TimeForPostnatal
                    SET @TimeForPostnatal = 0
                END
            END
        END
        ELSE
        BEGIN -- No shift in attendance date 
            -- get time in
            SELECT @InOfMachine = MIN(RecordTime)
            FROM @TimeRecord 
            WHERE InOutType = 0
                AND DATEDIFF(d, @attendaceDate, RecordTime) = 0

            -- get time out
            SELECT @OutOfMachine = MAX(RecordTime)
            FROM @TimeRecord 
            WHERE InOutType = 1
                AND DATEDIFF(d, @attendaceDate, RecordTime) = 0

            IF @InOfMachine > @OutOfMachine
            BEGIN
                SET @OutOfMachine = NULL
            END

            IF @RequestedCheckinTime IS NOT NULL
            BEGIN
                SET @InOfMachine = @RequestedCheckinTime
            END

            IF @RequestedCheckoutTime IS NOT NULL
            BEGIN
                SET @OutOfMachine = @RequestedCheckoutTime
            END

            IF (@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL)
            BEGIN
                SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine))/ 60.0 
            END
        END

        IF (@isOffDay = 1 OR @IsHoliday = 1 OR (@IsLeaveDay = 1 AND @LeaveDay = 1))
        BEGIN
            SET @Absence = 1
        END
        ELSE IF (@InOfMachine IS NULL AND @OutOfMachine IS NULL)
        BEGIN
            SET @Absence = 1
        END
        IF(@ShiftId IS NOT NULL OR @InOfMachine IS NOT NULL OR @OutOfMachine IS NOT NULL OR @IsLeaveDay = 1 OR @isOffDay = 1 OR @IsHoliday = 1)
        BEGIN
            -- insert or update
            IF (@onlyUpdate = 1 
                OR EXISTS 
                    (SELECT 1 
                        FROM dbo.EmployeeAttendance WITH (NOLOCK) 
                        WHERE EmpId = @empId 
                        AND DATEDIFF(d, AttendanceDate, @attendaceDate) = 0))
            BEGIN
                select 1, @onlyUpdate, @attendaceDate
                --update
                UPDATE dbo.EmployeeAttendance
                SET 
                    ShiftId = @ShiftId, 
                    InFixed = @InOfMachine,
                    OutFixed = @OutOfMachine,
                    ExpectedInTime = @ExpectedInTime,
                    ExpectedOutTime = @ExpectedOutTime,
                    LateIn  = @ActualLateIn,
                    EarlyOut = @ActualEarlyOut,
                    LeaveTypeId = @LeaveTypeId,
                    Absence = @Absence,
                    TotalWorkHours = @TotalWorkHours,
                    IsRightShift = @IsRightShift,
                    LeaveDay = @LeaveDay,
                    InsertDate = dbo.fnc_GetClientDateTime()
                WHERE EmpId = @empId AND DATEDIFF(d, AttendanceDate, @attendaceDate) = 0
            END
            ELSE
            BEGIN
                select 1, @onlyUpdate, @attendaceDate
                --insert
                INSERT INTO dbo.EmployeeAttendance
                (
                    EmpId,
                    AttendanceDate,
                    ShiftId,
                    InFixed,
                    OutFixed,
                    ExpectedInTime,
                    ExpectedOutTime,
                    LateIn,
                    EarlyOut,
                    LeaveTypeId,
                    Absence,
                    TotalWorkHours,
                    IsRightShift,
                    LeaveDay,
                    InsertDate
                )
                VALUES
                (
                    @empId, 
                    @attendaceDate,
                    @ShiftId,
                    @InOfMachine,
                    @OutOfMachine,
                    @ExpectedInTime,
                    @ExpectedOutTime,
                    @ActualLateIn,
                    @ActualEarlyOut,
                    @LeaveTypeId,
                    @Absence,
                    @TotalWorkHours,
                    @IsRightShift,
                    @LeaveDay,
                    dbo.fnc_GetClientDateTime()
                )            
            END
        END
        --ELSE
        --BEGIN
        --    DELETE 
        --    FROM dbo.EmployeeAttendance 
        --    WHERE EmpId = @empId AND DATEDIFF(d, AttendanceDate, @attendaceDate) = 0
        --END

        SET @i = @i + 1
        SET @attendaceDate = DATEADD(d, @i, @fromTime)
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForEmpId_new] 
    @empId INT
    ,@fromTime DATETIME
    ,@toTime DATETIME
    ,@onlyUpdate BIT = 0
AS
BEGIN
    DECLARE @RangeToScanTime INT
        ,@IsFlexibleTime BIT
        ,@DeductEarlyOut INT
        ,@BreakTime INT
        ,@i INT
        ,@WorkDate DATETIME
        ,@ShiftId INT
        ,@IsHoliday BIT
        ,@isOffDay BIT
        ,@IsLeaveDay BIT
        ,@LeaveTypeId INT
		,@LeaveTypeId2 INT
        ,@LateIn INT
        ,@EarlyOut INT
        ,@RequestedCheckinTime DATETIME
        ,@RequestedCheckoutTime DATETIME
        ,@BeginCheckInTime DATETIME
        ,@EndCheckInTime DATETIME
        ,@BeginCheckOutTime DATETIME
        ,@EndCheckOutTime DATETIME
        ,@InOfMachine DATETIME
        ,@OutOfMachine DATETIME
        ,@ExpectedInTime DATETIME
        ,@ExpectedOutTime DATETIME
        ,@Absence BIT
        ,@ActualLateIn INT
        ,@ActualEarlyOut INT
        ,@TotalWorkHours DECIMAL(18, 2)
        ,@BreakTimeMins INT
        ,@BeginBreakTime DATETIME
        ,@EndBreakTime DATETIME
        ,@LeaveDay DECIMAL(18, 1)
        ,@RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
        ,@adjustLateIn INT
        ,@adjustEarlyOut INT
        ,@TimeForPostnatal INT

    SET @i = 0
    SET @WorkDate = CONVERT(VARCHAR(10), @fromTime, 111)
	
    WHILE (DATEDIFF(d, @WorkDate, @toTime) >= 0)
    BEGIN
        SELECT @Absence = 0
            ,@InOfMachine = NULL
            ,@OutOfMachine = NULL
            ,@ExpectedOutTime = NULL
            ,@ExpectedInTime = NULL
            ,@ExpectedOutTime = NULL
            ,@ActualLateIn = 0
            ,@ActualEarlyOut = 0
            ,@RequestedCheckinTime = NULL
            ,@RequestedCheckoutTime = NULL
            ,@TotalWorkHours = NULL
            ,@BreakTimeMins = 0
            ,@ShiftId = NULL
            ,@LeaveTypeId = NULL
			,@LeaveTypeId2 = NULL
            ,@RequestLeaveTimeBegin = NULL
            ,@RequestLeaveTimeEnd = NULL
            ,@adjustLateIn = 0
            ,@adjustEarlyOut = 0
            ,@DeductEarlyOut = 0
            ,@BeginBreakTime = NULL
            ,@EndBreakTime = NULL
            ,@TimeForPostnatal = 0

        SELECT @WorkDate = WorkDate
            ,@ShiftId = ShiftId
            ,@IsHoliday = IsHoliday
            ,@isOffDay = IsOffDay
            ,@IsLeaveDay = IsLeaveDay
            ,@LeaveTypeId = LeaveTypeId
			,@LeaveTypeId2 = LeaveTypeId2
            ,@LateIn = ISNULL(LateIn, 0)
            ,@EarlyOut = ISNULL(EarlyOut, 0)
            ,@RequestedCheckinTime = RequestedCheckinTime
            ,@RequestedCheckoutTime = RequestedCheckoutTime
            ,@BeginCheckInTime = BeginCheckInTime
            ,@EndCheckInTime = EndCheckInTime
            ,@BeginCheckOutTime = BeginCheckOutTime
            ,@EndCheckOutTime = EndCheckOutTime
            ,@BreakTimeMins = BreakTimeMins
            ,@BeginBreakTime = BeginBreakTime
            ,@EndBreakTime = EndBreakTime
            ,@LeaveDay = LeaveDay
            ,@TimeForPostnatal = TimeForPostnatal
        FROM #EmpCalendar
        WHERE EmpId = @empId
            AND WorkDate = @WorkDate

        -- If Has Shift
        IF @ShiftId IS NOT NULL
        BEGIN
			IF (@LeaveTypeId = 9 OR @LeaveTypeId = 32 OR @LeaveTypeId2 = 9 OR @LeaveTypeId2 = 32)
			BEGIN
				EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShift_BusinessTrip] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
					,@LeaveTypeId = @LeaveTypeId
					,@LeaveTypeId2 = @LeaveTypeId2
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
			END
			ELSE IF (@ShiftId = 10) -- shift M1
            BEGIN
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM1] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
            ELSE IF (@ShiftId = 11) -- shift M2
            BEGIN
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM2] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
            ELSE IF (@ShiftId = 12) -- shift M3
            BEGIN
			
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM3] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
            ELSE -- shift # M1, M2, M3
            BEGIN
				
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShift] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@IsLeaveDay = @IsLeaveDay
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
        END
        ELSE
        BEGIN -- No shift in attendance date 
            EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForNoShift] @empId = @empId
                ,@WorkDate = @WorkDate
                ,@ShiftId = @ShiftId
                ,@IsLeaveDay = @IsLeaveDay
                ,@LeaveTypeId = @LeaveTypeId
                ,@LateIn = @LateIn
                ,@EarlyOut = @EarlyOut
                ,@RequestedCheckinTime = @RequestedCheckinTime
                ,@RequestedCheckoutTime = @RequestedCheckoutTime
                ,@BeginCheckInTime = @BeginCheckInTime
                ,@EndCheckInTime = @EndCheckInTime
                ,@BeginCheckOutTime = @BeginCheckOutTime
                ,@EndCheckOutTime = @EndCheckOutTime
                ,@BreakTimeMins = @BreakTimeMins
                ,@BeginBreakTime = @BeginBreakTime
                ,@EndBreakTime = @EndBreakTime
                ,@LeaveDay = @LeaveDay
                ,@TimeForPostnatal = @TimeForPostnatal
                ,@InOfMachine = @InOfMachine OUT
                ,@OutOfMachine = @OutOfMachine OUT
                ,@ExpectedInTime = @ExpectedInTime OUT
                ,@ExpectedOutTime = @ExpectedOutTime OUT
                ,@ActualLateIn = @ActualLateIn OUT
                ,@ActualEarlyOut = @ActualEarlyOut OUT
                ,@TotalWorkHours = @TotalWorkHours OUT
        END

        -- hau san 7h
        IF(@TimeForPostnatal > 0 AND @ActualLateIn > 0)
        BEGIN
            IF (@TimeForPostnatal > @ActualLateIn)
            BEGIN
                SET @TimeForPostnatal = @TimeForPostnatal - @ActualLateIn
                SET @ActualLateIn = 0
            END
            ELSE
            BEGIN
                SET @ActualLateIn = @ActualLateIn - @TimeForPostnatal
                SET @TimeForPostnatal = 0
            END
        END

        IF(@TimeForPostnatal > 0 AND @ActualEarlyOut > 0)
        BEGIN
            IF (@TimeForPostnatal > @ActualEarlyOut)
            BEGIN
                SET @TimeForPostnatal = @TimeForPostnatal - @ActualEarlyOut
                SET @ActualEarlyOut = 0
            END
            ELSE
            BEGIN
                SET @ActualEarlyOut = @ActualEarlyOut - @TimeForPostnatal
                SET @TimeForPostnatal = 0
            END
        END

        IF(@LateIn > 0 AND @ActualLateIn > 0)
        BEGIN
            IF (@LateIn > @ActualLateIn)
            BEGIN
                SET @ActualLateIn = 0
            END
            ELSE
            BEGIN
                SET @ActualLateIn = @ActualLateIn - @LateIn
            END
        END

        IF(@EarlyOut > 0 AND @ActualEarlyOut > 0)
        BEGIN
            IF (@EarlyOut > @ActualEarlyOut)
            BEGIN
                SET @ActualEarlyOut = 0
            END
            ELSE
            BEGIN
                SET @ActualEarlyOut = @ActualEarlyOut - @EarlyOut
            END
        END

        IF (
                @isOffDay = 1
                OR @IsHoliday = 1
                OR (
                    @IsLeaveDay = 1
                    AND @LeaveDay = 1
                    )
                )
        BEGIN
            SET @Absence = 1
        END
        ELSE IF (
                @InOfMachine IS NULL
                AND @OutOfMachine IS NULL
                )
        BEGIN
            SET @Absence = 1
        END

        IF (
                @ShiftId IS NOT NULL
                OR @InOfMachine IS NOT NULL
                OR @OutOfMachine IS NOT NULL
                OR @IsLeaveDay = 1
                OR @isOffDay = 1
                OR @IsHoliday = 1
                )
        BEGIN
            -- insert or update
            IF (
                    EXISTS (
                        SELECT 1
                        FROM dbo.EmployeeAttendance WITH (NOLOCK)
                        WHERE EmpId = @empId
                            AND DATEDIFF(d, AttendanceDate, @WorkDate) = 0
                        )
                    )
            BEGIN
                UPDATE dbo.EmployeeAttendance
                SET ShiftId = @ShiftId
                    ,InFixed = @InOfMachine
                    ,OutFixed = @OutOfMachine
                    ,ExpectedInTime = @ExpectedInTime
                    ,ExpectedOutTime = @ExpectedOutTime
                    ,LateIn = @ActualLateIn
                    ,EarlyOut = @ActualEarlyOut
                    ,LeaveTypeId = @LeaveTypeId
                    ,Absence = @Absence
                    ,TotalWorkHours = @TotalWorkHours
                    ,IsRightShift = NULL
                    ,LeaveDay = @LeaveDay
                    ,InsertDate = dbo.fnc_GetClientDateTime()
                WHERE EmpId = @empId
                    AND DATEDIFF(d, AttendanceDate, @WorkDate) = 0
            END
            ELSE
            BEGIN
				
                INSERT INTO dbo.EmployeeAttendance (
                    EmpId
                    ,AttendanceDate
                    ,ShiftId
                    ,InFixed
                    ,OutFixed
                    ,ExpectedInTime
                    ,ExpectedOutTime
                    ,LateIn
                    ,EarlyOut
                    ,LeaveTypeId
                    ,Absence
                    ,TotalWorkHours
                    ,IsRightShift
                    ,LeaveDay
                    ,InsertDate
                    )
                VALUES (
                    @empId
                    ,@WorkDate
                    ,@ShiftId
                    ,@InOfMachine
                    ,@OutOfMachine
                    ,@ExpectedInTime
                    ,@ExpectedOutTime
                    ,@ActualLateIn
                    ,@ActualEarlyOut
                    ,@LeaveTypeId
                    ,@Absence
                    ,@TotalWorkHours
                    ,NULL
                    ,@LeaveDay
                    ,dbo.fnc_GetClientDateTime()
                    )
            END
        END
        ELSE
        BEGIN
            DELETE
            FROM dbo.EmployeeAttendance
            WHERE EmpId = @empId
                AND DATEDIFF(d, AttendanceDate, @WorkDate) = 0
        END

		IF (@onlyUpdate = 1)
        BEGIN
            EXEC [dbo].[LateInEarlyOut_InsertTransaction] @empId, @WorkDate, @ActualLateIn, @ActualEarlyOut
        END

        SET @i = @i + 1
        SET @WorkDate = DATEADD(d, @i, @fromTime)
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForNoShift]
    @empId INT
    ,@WorkDate DATETIME
    ,@ShiftId INT
    ,@IsLeaveDay BIT
    ,@LeaveTypeId INT
    ,@LateIn INT
    ,@EarlyOut INT
    ,@RequestedCheckinTime DATETIME
    ,@RequestedCheckoutTime DATETIME
    ,@BeginCheckInTime DATETIME
    ,@EndCheckInTime DATETIME
    ,@BeginCheckOutTime DATETIME
    ,@EndCheckOutTime DATETIME
    ,@BreakTimeMins INT
    ,@BeginBreakTime DATETIME
    ,@EndBreakTime DATETIME
    ,@LeaveDay DECIMAL(18, 1)
    ,@TimeForPostnatal INT
    ,@InOfMachine DATETIME OUT
    ,@OutOfMachine DATETIME OUT
    ,@ExpectedInTime DATETIME OUT
    ,@ExpectedOutTime DATETIME OUT
    ,@ActualLateIn INT OUT
    ,@ActualEarlyOut INT OUT
    ,@TotalWorkHours DECIMAL(18, 2) OUT
AS
/*
Created: 20150729@Quinn.Vo
Task: Calculate time for no shift.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    DECLARE @DeductEarlyOut INT
        ,@RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
        ,@RangeToScanTime INT
        ,@adjustLateIn INT
        ,@adjustEarlyOut INT

    SELECT @RangeToScanTime = 480, @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime

    -- get time in
    SELECT @InOfMachine = MIN(RecordTime)
    FROM #TimeRecord
    WHERE EmpId = @empId
        AND InOutType = 0
        AND DATEDIFF(d, @WorkDate, RecordTime) = 0

    IF @RequestedCheckinTime IS NOT NULL
    BEGIN
        SET @InOfMachine = @RequestedCheckinTime
    END

    -- get time out
    SELECT @OutOfMachine = MIN(RecordTime)
    FROM #TimeRecord 
    WHERE EmpId = @empId
        AND InOutType = 1
        AND DATEDIFF(d, @WorkDate, RecordTime) = 0
        AND (@InOfMachine IS NULL OR DATEDIFF(D, RecordTime, DATEADD(MI, 2, @InOfMachine)) >= 0)

    IF @RequestedCheckoutTime IS NOT NULL
    BEGIN
        SET @OutOfMachine = @RequestedCheckoutTime
    END

    IF (@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL)
    BEGIN
        SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine))/ 60.0 
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShift]
	@empId INT
	,@WorkDate DATETIME
	,@IsLeaveDay BIT
	,@RequestedCheckinTime DATETIME
	,@RequestedCheckoutTime DATETIME
	,@BeginCheckInTime DATETIME
	,@EndCheckInTime DATETIME
	,@BeginCheckOutTime DATETIME
	,@EndCheckOutTime DATETIME
	,@BreakTimeMins INT
	,@BeginBreakTime DATETIME
	,@EndBreakTime DATETIME
	,@LeaveDay DECIMAL(18, 1)
	,@InOfMachine DATETIME OUT
	,@OutOfMachine DATETIME OUT
	,@ExpectedInTime DATETIME OUT
	,@ExpectedOutTime DATETIME OUT
	,@ActualLateIn INT OUT
	,@ActualEarlyOut INT OUT
	,@TotalWorkHours DECIMAL(18, 2) OUT
AS
/*
Created: 20150729@Quinn.Vo
Task: Calculate time for shift #M1, M2, M3.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	DECLARE @DeductEarlyOut INT
		,@RequestLeaveTimeBegin DATETIME
		,@RequestLeaveTimeEnd DATETIME
		,@RangeToScanTime INT
		,@adjustLateIn INT
		,@adjustEarlyOut INT
		,@AdjustTotalHours DECIMAL(5,2) = 0

	SELECT @RangeToScanTime = 480, @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime

	-- get time in
	SELECT @InOfMachine = MIN(RecordTime)
	FROM #TimeRecord
	WHERE EmpId = @empId
		AND InOutType = 0
		AND RecordTime BETWEEN DATEADD(mi, -@RangeToScanTime, @ExpectedInTime) AND @ExpectedOutTime

	IF (@InOfMachine IS NULL)
	BEGIN
		SELECT @InOfMachine = MIN(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType IS NULL
			AND DATEDIFF(MI, DATEADD(HH, (DATEDIFF(HH, @ExpectedInTime, @ExpectedOutTime))/2 + 1, @ExpectedInTime), RecordTime) <= 0
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0

	END

	IF @RequestedCheckinTime IS NOT NULL
	BEGIN
		SET @InOfMachine = @RequestedCheckinTime
	END

	-- get time out
	SELECT @OutOfMachine = MIN(RecordTime)
	FROM #TimeRecord 
	WHERE EmpId = @empId
		AND InOutType = 1
		AND RecordTime BETWEEN @ExpectedInTime AND DATEADD(mi, @RangeToScanTime, @ExpectedOutTime)
		AND (@InOfMachine IS NULL OR DATEDIFF(D, RecordTime, DATEADD(MI, 2, @InOfMachine)) >= 0)

	IF (@OutOfMachine IS NULL)
	BEGIN
		SELECT @OutOfMachine = MIN(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType IS NULL
			AND DATEDIFF(MI, DATEADD(HH, -1, @ExpectedOutTime), RecordTime) >= 0
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0


		IF (DATEDIFF(HH, @InOfMachine, @OutOfMachine) < 3)
		BEGIN
			SELECT @OutOfMachine = NULL 
		END
	END

	IF (@OutOfMachine IS NULL)
	BEGIN
		SELECT @OutOfMachine = MAX(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType IS NULL
			AND DATEDIFF(MI, DATEADD(HH, -1, @ExpectedOutTime), RecordTime) < 0
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0

		IF (DATEDIFF(HH, @InOfMachine, @OutOfMachine) < 3)
		BEGIN
			SELECT @OutOfMachine = NULL 
		END
	END


	IF @RequestedCheckoutTime IS NOT NULL
	BEGIN
		SET @OutOfMachine = @RequestedCheckoutTime
	END

	IF (DATEDIFF(MI, @OutOfMachine, @ExpectedOutTime) > 0)
	BEGIN
		SET @ActualEarlyOut = DATEDIFF (MI, @OutOfMachine, @ExpectedOutTime)	
	END
	IF (DATEDIFF(MI, @ExpectedINTime, @InOfMachine) > 0) 
	BEGIN
		SET @ActualLateIn = DATEDIFF (MI, @ExpectedINTime, @InOfMachine) 
	END
			
	IF (@InOfMachine IS NOT NULL ) --AND @OutOfMachine IS NOT NULL)
	BEGIN
		IF(@IsLeaveDay = 1 and @LeaveDay = 0.5)
		BEGIN 
			SELECT @RequestLeaveTimeBegin = FromTime, @RequestLeaveTimeEnd = ToTime
			FROM #LeaveRequestHalfDay
			WHERE EmpId = @empId AND OnDate = @WorkDate
			IF(@RequestLeaveTimeBegin IS NOT NULL AND @RequestLeaveTimeEnd IS NOT NULL)
			BEGIN
				IF(DATEDIFF(mi, @ExpectedInTime, @RequestLeaveTimeBegin) < 150 AND DATEDIFF(mi, @ExpectedInTime, @RequestLeaveTimeBegin) > -150)
				BEGIN
					SET @adjustLateIn = DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)
					SET @ActualLateIn = @ActualLateIn - @adjustLateIn
					IF @ActualLateIn < 0
					BEGIN
						SET @ActualLateIn = 0
					END
				END
				ELSE IF (DATEDIFF(mi, @ExpectedOutTime, @RequestLeaveTimeEnd) < 150 AND DATEDIFF(mi, @ExpectedOutTime, @RequestLeaveTimeEnd) > -150)
				BEGIN
					SET @adjustEarlyOut = DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)
					SET @ActualEarlyOut = @ActualEarlyOut - @adjustEarlyOut
					IF @ActualEarlyOut < 0
					BEGIN
						SET @ActualEarlyOut = 0
					END
				END
			END
		END

		IF (@InOfMachine < @BeginCheckInTime)
		BEGIN
			SELECT @AdjustTotalHours = DATEDIFF(MI, @InOfMachine, @BeginCheckInTime) / 60.0
		END

		IF (@OutOfMachine IS NOT NULL)
		BEGIN
			SET @TotalWorkHours =  (DATEDIFF(mi, @InOfMachine, @OutOfMachine) - ISNULL(@BreakTimeMins, 0))/ 60.0 - @AdjustTotalHours
		END

		IF (@TotalWorkHours < 0)
		BEGIN
			SELECT @TotalWorkHours = 0
		END
	END

	--SELECT @InOfMachine
 --                   ,@OutOfMachine 
 --                   ,@ExpectedInTime 
 --                   ,@ExpectedOutTime 
 --                   ,@ActualLateIn 
 --                   ,@ActualEarlyOut 
 --                   ,@TotalWorkHours 
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShift_BusinessTrip] 
    @empId INT
    ,@WorkDate DATETIME
    ,@RequestedCheckinTime DATETIME
    ,@RequestedCheckoutTime DATETIME
    ,@BeginCheckInTime DATETIME
    ,@EndCheckInTime DATETIME
    ,@BeginCheckOutTime DATETIME
    ,@EndCheckOutTime DATETIME
    ,@BreakTimeMins INT
    ,@BeginBreakTime DATETIME
    ,@EndBreakTime DATETIME
    ,@LeaveDay DECIMAL(18, 1)
	,@LeaveTypeId INT = NULL
	,@LeaveTypeId2 INT = NULL
    ,@InOfMachine DATETIME OUT
    ,@OutOfMachine DATETIME OUT
    ,@ExpectedInTime DATETIME OUT
    ,@ExpectedOutTime DATETIME OUT
    ,@ActualLateIn INT OUT
    ,@ActualEarlyOut INT OUT
    ,@TotalWorkHours DECIMAL(18, 2) OUT
AS
/*
Created: 20180919@Quinn.Vo
Task: Calculate time for shift M1 & Business Trip.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    DECLARE @RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
		,@TempTimeIn DATETIME
		,@TempTimeOut DATETIME
        ,@IsUserStandby BIT
		,@AdjustTotalHours DECIMAL(5,2) = 0

    SELECT @IsUserStandby = 0
    IF (EXISTS(SELECT 1 FROM dbo.Employee WITH (NOLOCK) WHERE GroupId IN (61, 59, 46)))
    BEGIN
        SET @IsUserStandby = 1
    END

	SELECT @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime

    IF @RequestedCheckinTime IS NULL
    BEGIN
        SELECT @InOfMachine = MIN(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType = 0
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0

		IF (@InOfMachine IS NULL)
		BEGIN
			SELECT @InOfMachine = MIN(RecordTime)
			FROM #TimeRecord
			WHERE EmpID = @empId
				AND InOutType IS NULL
				AND DATEDIFF(MI, DATEADD(HH, (DATEDIFF(HH, @ExpectedInTime, @ExpectedOutTime))/2 + 1, @ExpectedInTime), RecordTime) <= 0
				AND DATEDIFF(d, @WorkDate, RecordTime) = 0
		END
    END
	ELSE
	BEGIN
		SET @InOfMachine = @RequestedCheckinTime
	END

    IF @RequestedCheckoutTime IS NULL
    BEGIN
        SELECT @OutOfMachine = MIN(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType = 1
			-- AND DATEDIFF(d, @WorkDate, RecordTime) = 0
			AND DATEDIFF(MI, DATEADD(HH, 5, @WorkDate), RecordTime) >= 0
			AND ((@IsUserStandby = 0 AND DATEDIFF(MI, RecordTime, DATEADD(HH, 5, DATEADD(D, 1, @WorkDate))) >= 0)
				OR (@IsUserStandby = 1 AND DATEDIFF(MI, RecordTime, DATEADD(HH, 9, DATEADD(D, 1, @WorkDate))) >= 0) -- to 9h
			)
			AND (
				@InOfMachine IS NULL
				OR RecordTime > DATEADD(MI, 2, @InOfMachine)
				)

		IF (@OutOfMachine IS NULL)
		BEGIN
			SELECT @OutOfMachine = MIN(RecordTime)
			FROM #TimeRecord
			WHERE EmpID = @empId
				AND InOutType IS NULL
				AND DATEDIFF(MI, DATEADD(HH, -1, @ExpectedOutTime), RecordTime) >= 0
				AND DATEDIFF(d, @WorkDate, RecordTime) = 0


			IF (DATEDIFF(HH, @InOfMachine, @OutOfMachine) < 3)
			BEGIN
				SELECT @OutOfMachine = NULL 
			END
		END

		IF (@OutOfMachine IS NULL)
		BEGIN
			SELECT @OutOfMachine = MAX(RecordTime)
			FROM #TimeRecord
			WHERE EmpID = @empId
				AND InOutType IS NULL
				AND DATEDIFF(MI, DATEADD(HH, -1, @ExpectedOutTime), RecordTime) < 0
				AND DATEDIFF(d, @WorkDate, RecordTime) = 0

			IF (DATEDIFF(HH, @InOfMachine, @OutOfMachine) < 3)
			BEGIN
				SELECT @OutOfMachine = NULL 
			END
		END
    END
	ELSE
	BEGIN
		SET @OutOfMachine = @RequestedCheckoutTime
	END
	
    SELECT @RequestLeaveTimeBegin = FromTime
        ,@RequestLeaveTimeEnd = ToTime
		,@TempTimeIn = FromTime
		,@TempTimeOut = ToTime
    FROM #LeaveRequestHalfDay
    WHERE EmpId = @empId
        AND OnDate = @WorkDate

		IF(ISNULL(@LeaveDay, 0) = 1)
		BEGIN
			
			IF ((@LeaveTypeId <> 9 AND @LeaveTypeId <> 32)
				OR (@LeaveTypeId2 IS NOT NULL AND @LeaveTypeId2 <> 9 AND @LeaveTypeId2 <> 32)
				)
			BEGIN
				SELECT @TotalWorkHours = 4.0
			END
			ELSE
			BEGIN
				IF(@InOfMachine IS NULL OR @OutOfMachine IS NULL)
				BEGIN
					SELECT @TotalWorkHours = ((DATEDIFF(mi, @EndCheckInTime, @EndCheckOutTime) - @BreakTimeMins) / 60.0)
				END
				ELSE
				BEGIN
					SELECT @TotalWorkHours = ((DATEDIFF(mi, @InOfMachine, @OutOfMachine) - @BreakTimeMins) / 60.0)
					IF(@TotalWorkHours < 8)
					BEGIN
						SELECT @TotalWorkHours = 8, @ActualLateIn = 0, @ActualEarlyOut = 0
					END
				END
			END
		END
		ELSE
		BEGIN
			--0.5 day
			
			IF (@InOfMachine IS NOT NULL)
			BEGIN
				-- Get @ExpectedInTime, @ExpectedOutTime
				IF (@RequestLeaveTimeBegin > @InOfMachine)
				BEGIN
					SELECT @TempTimeIn = @InOfMachine
				END
				
				IF (@OutOfMachine IS NOT NULL AND @RequestLeaveTimeEnd < @OutOfMachine)
				BEGIN
					SELECT @TempTimeOut = @OutOfMachine
				END

				IF (ISNULL(@BreakTimeMins, 0) = 0)
				BEGIN
					SELECT @ExpectedInTime = @EndCheckInTime
						,@ExpectedOutTime = @EndCheckOutTime
				END
				ELSE
				BEGIN
					IF (DATEDIFF(MI, @TempTimeIn, @BeginCheckInTime) >= 0) -- <8h
					BEGIN
						SELECT @ExpectedInTime = @BeginCheckInTime
							,@ExpectedOutTime = @BeginCheckOutTime
					END
					ELSE IF (DATEDIFF(MI, @TempTimeIn, @EndCheckInTime) >= 0) -- 8->9h
					BEGIN
						SELECT @ExpectedInTime = @TempTimeIn
							,@ExpectedOutTime = DATEADD(HH, 9, @TempTimeIn)
					END
					ELSE IF (DATEDIFF(MI, @TempTimeIn, DATEADD(HH, 1, @EndCheckInTime)) >= 0) -- 9h->10h
					BEGIN
						SELECT @ExpectedInTime = @EndCheckInTime
							,@ExpectedOutTime = @EndCheckOutTime
					END
					ELSE -- >10h
					BEGIN
						SELECT @ExpectedInTime = @EndCheckInTime
							,@ExpectedOutTime = @EndCheckOutTime
					END
				END
				-- calculate total working hours, late in/early out
				IF (@TempTimeOut < @ExpectedOutTime)
				BEGIN
					SET @ActualEarlyOut = DATEDIFF(MI, @TempTimeOut, @ExpectedOutTime)
				END

				IF (@TempTimeIn > @ExpectedInTime)
				BEGIN
					SET @ActualLateIn = DATEDIFF(MI, @ExpectedInTime, @TempTimeIn)
				END

				SET @TotalWorkHours = DATEDIFF(mi, @TempTimeIn, @TempTimeOut) / 60.0

				IF (@OutOfMachine IS NOT NULL)
				BEGIN
					IF (ISNULL(@BreakTimeMins, 0) = 0)
					BEGIN
						IF (@RequestLeaveTimeEnd < @InOfMachine)
						BEGIN
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@OutOfMachine < @RequestLeaveTimeBegin)
						BEGIN
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE
						BEGIN
							SELECT @TotalWorkHours = DATEDIFF(mi, @TempTimeIn, @TempTimeOut) / 60.0
						END
					END
					ELSE
					BEGIN
						IF(@RequestLeaveTimeEnd < @InOfMachine)
					BEGIN
						IF(@EndBreakTime <@RequestLeaveTimeBegin)
						BEGIN
							--1: @BeginBreakTime < @EndBreakTime < @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@RequestLeaveTimeBegin >= @BeginBreakTime AND @RequestLeaveTimeBegin <= @EndBreakTime
							AND @EndBreakTime < @RequestLeaveTimeEnd)
						BEGIN
							-- 2: @BeginBreakTime < @RequestLeaveTimeBegin < @EndBreakTime < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
								- DATEDIFF(mi, @RequestLeaveTimeBegin, @EndBreakTime)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd
							AND @EndBreakTime >= @RequestLeaveTimeBegin AND @EndBreakTime <= @RequestLeaveTimeEnd)
						BEGIN
							-- 3: @RequestLeaveTimeBegin < @BeginBreakTime < @EndBreakTime < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd 
							AND @EndBreakTime >= @RequestLeaveTimeEnd AND @EndBreakTime <= @InOfMachine)
						BEGIN
							-- 4: @RequestLeaveTimeBegin < @BeginBreakTime < @RequestLeaveTimeEnd < @EndBreakTime < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @EndBreakTime, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @EndBreakTime, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeEnd AND @BeginBreakTime < @InOfMachine
							AND @EndBreakTime >= @RequestLeaveTimeEnd AND @EndBreakTime < @InOfMachine)
						BEGIN
							-- 5: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @BeginBreakTime < @EndBreakTime < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeEnd AND @BeginBreakTime <= @InOfMachine
							AND @EndBreakTime >= @InOfMachine AND @EndBreakTime <= @OutOfMachine)
						BEGIN
							--6: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @BeginBreakTime < @InOfMachine < @EndBreakTime < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @RequestLeaveTimeEnd, @BeginBreakTime)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @BeginBreakTime)
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine
							AND @EndBreakTime >= @InOfMachine AND @EndBreakTime <= @OutOfMachine)
						BEGIN
							-- 7: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @BeginBreakTime < @EndBreakTime < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine
							AND @EndBreakTime >= @OutOfMachine)
						BEGIN
							-- 8: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @BeginBreakTime < @OutOfMachine < @EndBreakTime 
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine) 
								- DATEDIFF(mi, @BeginBreakTime, @OutOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime > @OutOfMachine)
						BEGIN
							--9: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine < @BeginBreakTime < @EndBreakTime
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @InOfMachine)
						END
						ELSE IF (@BeginBreakTime <= @RequestLeaveTimeBegin AND @EndBreakTime >= @RequestLeaveTimeEnd AND @EndBreakTime <= @InOfMachine)
						BEGIN
							--10: @BeginBreakTime < @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @EndBreakTime < @InOfMachine < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @RequestLeaveTimeBegin, @InOfMachine)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @EndBreakTime, @InOfMachine) - DATEDIFF(mi, @BeginBreakTime, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd
							AND @EndBreakTime >= @InOfMachine AND @EndBreakTime <= @OutOfMachine)
						BEGIN
							--11: @RequestLeaveTimeBegin < @BeginBreakTime < @RequestLeaveTimeEnd < @InOfMachine < @EndBreakTime < @OutOfMachine
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeEnd AND @BeginBreakTime <= @InOfMachine
							AND @EndBreakTime >= @OutOfMachine)
						BEGIN
							--12: @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @BeginBreakTime < @InOfMachine < @OutOfMachine < @EndBreakTime
							SELECT @TotalWorkHours = (DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)) / 60.0
								,@ActualLateIn = @ActualLateIn + DATEDIFF(mi, @RequestLeaveTimeEnd, @BeginBreakTime)
						END
						ELSE IF (@BeginBreakTime <= @RequestLeaveTimeBegin AND @EndBreakTime >= @InOfMachine AND @EndBreakTime <= @OutOfMachine)
						BEGIN
							--13: @BeginBreakTime < @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @EndBreakTime < @OutOfMachine 
							SELECT @TotalWorkHours = (DATEDIFF(mi, @EndBreakTime, @OutOfMachine)) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd
							AND @EndBreakTime >= @OutOfMachine)
						BEGIN
							--14: @RequestLeaveTimeBegin < @BeginBreakTime < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine < @EndBreakTime
							SELECT @TotalWorkHours = (DATEDIFF(mi, @RequestLeaveTimeBegin, @BeginBreakTime)) / 60.0
						END
						ELSE
						BEGIN
							--15: @BeginBreakTime < @RequestLeaveTimeBegin < @RequestLeaveTimeEnd < @InOfMachine < @OutOfMachine < @EndBreakTime
							SELECT @TotalWorkHours = 0
						END
					END
					ELSE IF (@OutOfMachine < @RequestLeaveTimeBegin)
					BEGIN
						IF(@EndBreakTime <@InOfMachine)
						BEGIN
							-- 1
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime <= @InOfMachine AND @InOfMachine <= @EndBreakTime
							AND @EndBreakTime < @OutOfMachine)
						BEGIN
							-- 2
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
								- DATEDIFF(mi, @InOfMachine, @EndBreakTime)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine
							AND @EndBreakTime >= @InOfMachine AND @EndBreakTime <= @OutOfMachine)
						BEGIN
							-- 3
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine 
							AND @EndBreakTime >= @OutOfMachine AND @EndBreakTime <= @RequestLeaveTimeBegin)
						BEGIN
							-- 4
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @EndBreakTime, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @EndBreakTime, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @OutOfMachine AND @BeginBreakTime < @RequestLeaveTimeBegin
							AND @EndBreakTime >= @OutOfMachine AND @EndBreakTime < @RequestLeaveTimeBegin)
						BEGIN
							-- 5
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(MI, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(MI, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @OutOfMachine AND @BeginBreakTime <= @RequestLeaveTimeBegin
							AND @EndBreakTime >= @RequestLeaveTimeBegin AND @EndBreakTime <= @RequestLeaveTimeEnd)
						BEGIN
							--6
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @OutOfMachine, @BeginBreakTime)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @BeginBreakTime)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd
							AND @EndBreakTime >= @RequestLeaveTimeBegin AND @EndBreakTime <= @RequestLeaveTimeEnd)
						BEGIN
							-- 7
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @RequestLeaveTimeBegin AND @BeginBreakTime <= @RequestLeaveTimeEnd
							AND @EndBreakTime >= @RequestLeaveTimeEnd)
						BEGIN
							-- 8
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin) 
								- DATEDIFF(mi, @BeginBreakTime, @RequestLeaveTimeEnd)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime > @RequestLeaveTimeEnd)
						BEGIN
							--9	
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime <= @InOfMachine AND @EndBreakTime >= @OutOfMachine AND @EndBreakTime <= @RequestLeaveTimeBegin)
						BEGIN
							--10
							SELECT @TotalWorkHours = (DATEDIFF(mi, @RequestLeaveTimeBegin, @RequestLeaveTimeEnd)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @RequestLeaveTimeBegin)
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine
							AND @EndBreakTime >= @RequestLeaveTimeBegin AND @EndBreakTime <= @RequestLeaveTimeEnd)
						BEGIN
							--11
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @OutOfMachine AND @BeginBreakTime <= @RequestLeaveTimeBegin
							AND @EndBreakTime >= @RequestLeaveTimeEnd)
						BEGIN
							--12
							SELECT @TotalWorkHours = (DATEDIFF(mi, @InOfMachine, @OutOfMachine)) / 60.0
								,@ActualEarlyOut = @ActualEarlyOut + DATEDIFF(mi, @OutOfMachine, @BeginBreakTime)
						END
						ELSE IF (@BeginBreakTime <= @InOfMachine AND @EndBreakTime >= @RequestLeaveTimeBegin AND @EndBreakTime <= @RequestLeaveTimeEnd)
						BEGIN
							--13
							SELECT @TotalWorkHours = (DATEDIFF(mi, @EndBreakTime, @RequestLeaveTimeEnd)) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @InOfMachine AND @BeginBreakTime <= @OutOfMachine
							AND @EndBreakTime >= @RequestLeaveTimeEnd)
						BEGIN
							--14
							SELECT @TotalWorkHours = (DATEDIFF(mi, @InOfMachine, @BeginBreakTime)) / 60.0
						END
						ELSE
						BEGIN
							--15
							SELECT @TotalWorkHours = 0
						END
					END
					ELSE
					BEGIN
						-- 6 case
						IF (@BeginBreakTime <= @TempTimeIn AND @EndBreakTime >= @TempTimeIn AND @EndBreakTime <= @TempTimeOut)
						BEGIN
							-- 2:
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(MI, @TempTimeIn, @EndBreakTime)) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @TempTimeIn AND @BeginBreakTime <= @TempTimeOut
							AND @EndBreakTime >= @TempTimeIn AND @EndBreakTime <= @TempTimeOut)
						BEGIN
							--3:
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - @BreakTimeMins) / 60.0
						END
						ELSE IF (@BeginBreakTime >= @TempTimeIn AND @BeginBreakTime <= @TempTimeOut
							AND @EndBreakTime >= @TempTimeOut)
						BEGIN
							-- 4
							SELECT @TotalWorkHours = (DATEDIFF(mi, @TempTimeIn, @TempTimeOut) - DATEDIFF(MI, @BeginBreakTime, @TempTimeOut)) / 60.0
						END
						ELSE IF (@BeginBreakTime <= @TempTimeIn AND @EndBreakTime >= @TempTimeOut)
						BEGIN
							--6
							SELECT @TotalWorkHours = 0
						END
						-- case 1&5: nothing
					END
					END
				END
			END

			IF (@OutOfMachine IS NULL)
			BEGIN
				SELECT @ActualEarlyOut = 0, @TotalWorkHours = 0
			END
		END

		IF (@TempTimeIn IS NULL)
		BEGIN
			SELECT @TempTimeIn = @InOfMachine
		END

		IF (@TempTimeIn < @BeginCheckInTime)
		BEGIN
			SELECT @AdjustTotalHours = DATEDIFF(MI, @TempTimeIn, @BeginCheckInTime) / 60.0
		END

		SELECT @TotalWorkHours = @TotalWorkHours - @AdjustTotalHours
		IF (@TotalWorkHours < 0)
		BEGIN
			SELECT @TotalWorkHours = 0
		END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM1] 
    @empId INT
    ,@WorkDate DATETIME
    ,@RequestedCheckinTime DATETIME
    ,@RequestedCheckoutTime DATETIME
    ,@BeginCheckInTime DATETIME
    ,@EndCheckInTime DATETIME
    ,@BeginCheckOutTime DATETIME
    ,@EndCheckOutTime DATETIME
    ,@BreakTimeMins INT
    ,@BeginBreakTime DATETIME
    ,@EndBreakTime DATETIME
    ,@LeaveDay DECIMAL(18, 1)
    ,@InOfMachine DATETIME OUT
    ,@OutOfMachine DATETIME OUT
    ,@ExpectedInTime DATETIME OUT
    ,@ExpectedOutTime DATETIME OUT
    ,@ActualLateIn INT OUT
    ,@ActualEarlyOut INT OUT
    ,@TotalWorkHours DECIMAL(18, 2) OUT
AS
/*
Created: 20150729@Quinn.Vo
Task: Calculate time for shift M1.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    DECLARE @DeductEarlyOut INT
        ,@BreakTime INT
        ,@RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
        ,@IsUserStandby BIT

    SELECT @DeductEarlyOut = 0, @IsUserStandby = 0
    IF (EXISTS(SELECT 1 FROM Employee WITH (NOLOCK) WHERE GroupId IN (61, 59, 46)))
    BEGIN
        SET @IsUserStandby = 1
    END

    SELECT @InOfMachine = MIN(RecordTime)
    FROM #TimeRecord
    WHERE EmpID = @empId
        AND InOutType = 0
        AND DATEDIFF(d, @WorkDate, RecordTime) = 0

	IF (@InOfMachine IS NULL)
	BEGIN
		SELECT @InOfMachine = MIN(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType IS NULL
			AND DATEDIFF(HH, @WorkDate, DATEADD(MI, 1, RecordTime)) < 14
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0

	END

    IF @RequestedCheckinTime IS NOT NULL
    BEGIN
        SET @InOfMachine = @RequestedCheckinTime
    END

    SELECT @OutOfMachine = MIN(RecordTime)
    FROM #TimeRecord
    WHERE EmpID = @empId
        AND InOutType = 1
        -- AND DATEDIFF(d, @WorkDate, RecordTime) = 0
        AND DATEDIFF(MI, DATEADD(HH, 5, @WorkDate), RecordTime) >= 0
        AND ((@IsUserStandby = 0 AND DATEDIFF(MI, RecordTime, DATEADD(HH, 5, DATEADD(D, 1, @WorkDate))) >= 0)
            OR (@IsUserStandby = 1 AND DATEDIFF(MI, RecordTime, DATEADD(HH, 9, DATEADD(D, 1, @WorkDate))) >= 0) -- to 9h
        )
        AND (
            @InOfMachine IS NULL
            OR RecordTime > DATEADD(MI, 2, @InOfMachine)
            )
	
	IF (@OutOfMachine IS NULL)
	BEGIN
		SELECT @OutOfMachine = MIN(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType IS NULL
			AND DATEDIFF(HH, @WorkDate, RecordTime) >= 17
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0
	END

	IF (@OutOfMachine IS NULL)
	BEGIN
		SELECT @OutOfMachine = MAX(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType IS NULL
			AND DATEDIFF(HH, @WorkDate, RecordTime) >= 11
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0

		IF (DATEDIFF(HH, @InOfMachine, @OutOfMachine) < 3)
		BEGIN
			SELECT @OutOfMachine = NULL 
		END
	END

    IF @RequestedCheckoutTime IS NOT NULL
    BEGIN
        SET @OutOfMachine = @RequestedCheckoutTime
    END
    
    SELECT @RequestLeaveTimeBegin = FromTime
        ,@RequestLeaveTimeEnd = ToTime
    FROM #LeaveRequestHalfDay
    WHERE EmpId = @empId
        AND OnDate = @WorkDate
	
    IF (@InOfMachine IS NOT NULL)
    BEGIN
        IF (ISNULL(@LeaveDay, 0) = 0)
        BEGIN
            IF (DATEDIFF(MI, @InOfMachine, @BeginCheckInTime) >= 0) -- <8h
            BEGIN
                SELECT @ExpectedInTime = @BeginCheckInTime
                    ,@ExpectedOutTime = @BeginCheckOutTime
            END
            ELSE IF (DATEDIFF(MI, @InOfMachine, @EndCheckInTime) >= 0) -- 8->9h
            BEGIN
                SELECT @ExpectedInTime = @InOfMachine
                    ,@ExpectedOutTime = DATEADD(HH, 9, @InOfMachine)
            END
            ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(HH, 1, @EndCheckInTime)) >= 0) -- 9h->10h
            BEGIN
                SELECT @ExpectedInTime = @EndCheckInTime
                    ,@ExpectedOutTime = @EndCheckOutTime
            END
            ELSE -- >10h
            BEGIN
                SELECT @ExpectedInTime = @EndCheckInTime
                    ,@ExpectedOutTime = @EndCheckOutTime
            END
        END
        ELSE IF (@LeaveDay = 0.5)
        BEGIN
                IF (DATEDIFF(MI, @InOfMachine, @BeginCheckInTime) >= 0) -- <8h
                BEGIN
                    SELECT @ExpectedInTime = @BeginCheckInTime
                        ,@ExpectedOutTime = DATEADD(HH, 4, @BeginCheckInTime)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, @EndCheckInTime) >= 0) -- 8->9h
                BEGIN
                    SELECT @ExpectedInTime = @InOfMachine
                        ,@ExpectedOutTime = DATEADD(HH, 5, @InOfMachine)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(HH, 1, @EndCheckInTime)) >= 0) -- 9h->10h
                BEGIN
                    IF (@RequestLeaveTimeBegin < @BeginBreakTime)
                    BEGIN
                        SELECT @ExpectedInTime = @EndBreakTime
                            ,@ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                    END
                    ELSE
                    BEGIN
                        SELECT @ExpectedInTime = @EndCheckInTime
                            ,@ExpectedOutTime = DATEADD(HH, 5, @EndCheckInTime)
                    END
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, @EndBreakTime) >= 0) -- 10h->13h
                BEGIN
                    SELECT @ExpectedInTime = @EndBreakTime
                        ,@ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(HH, 5, @EndCheckInTime)) >= 0) -- 13h-->14h00
                BEGIN
                    SELECT @ExpectedInTime = @InOfMachine
                        ,@ExpectedOutTime = DATEADD(HH, 4, @InOfMachine)
                END
                ELSE -- >14h00
                BEGIN
                    SELECT @ExpectedInTime = DATEADD(HH, 4, @EndCheckInTime)
                        ,@ExpectedOutTime = DATEADD(HH, 8, @EndCheckInTime)
                END
        END

        IF (
                @OutOfMachine < @EndBreakTime
                AND @ExpectedOutTime > @EndBreakTime
                )
        BEGIN
            SELECT @DeductEarlyOut = CASE 
                    WHEN DATEDIFF(MI, @OutOfMachine, @EndBreakTime) > @BreakTimeMins THEN @BreakTimeMins
                    ELSE DATEDIFF(MI, @OutOfMachine, @EndBreakTime)
                    END
        END
        
        IF (@OutOfMachine < @ExpectedOutTime)
        BEGIN
            SET @ActualEarlyOut = DATEDIFF(MI, @OutOfMachine, @ExpectedOutTime) - @DeductEarlyOut
        END

        IF (@InOfMachine > @ExpectedINTime)
        BEGIN
            SET @ActualLateIn = DATEDIFF(MI, @ExpectedINTime, @InOfMachine)
        END
    END
    ELSE
    BEGIN
        SELECT @ExpectedInTime = @EndCheckInTime
            ,@ExpectedOutTime = @EndCheckOutTime
    END

    -- Break time for total hours
    IF (@InOfMachine IS NOT NULL AND @OutOfMachine IS NOT NULL AND ISNULL(@LeaveDay, 0) <> 1)
    BEGIN
        SET @BreakTime = 0

        IF (@InOfMachine < @BeginCheckInTime)
        BEGIN
            SET @BreakTime = DATEDIFF(MI, @InOfMachine, @BeginCheckInTime)
        END

        IF (
                @InOfMachine <= @BeginBreakTime
                AND @OutOfMachine >= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + @BreakTimeMins
        END
        ELSE IF (
                @InOfMachine >= @BeginBreakTime
                AND @OutOfMachine <= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = 1440
        END
        ELSE IF (
                @InOfMachine >= @BeginBreakTime
                AND @InOfMachine <= @EndBreakTime
                AND @OutOfMachine >= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + (DATEDIFF(MI, @InOfMachine, @EndBreakTime))
        END
        ELSE IF (
                @InOfMachine <= @BeginBreakTime
                AND @OutOfMachine >= @BeginBreakTime
                AND @OutOfMachine <= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + (DATEDIFF(MI, @BeginBreakTime, @OutOfMachine))
        END

        SET @TotalWorkHours = ((DATEDIFF(mi, @InOfMachine, @OutOfMachine) - @BreakTime) / 60.0)
    END

	IF (@TotalWorkHours < 0)
    BEGIN
        SET @TotalWorkHours = 0
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM2] 
    @empId INT
    ,@WorkDate DATETIME
    ,@RequestedCheckinTime DATETIME
    ,@RequestedCheckoutTime DATETIME
    ,@BeginCheckInTime DATETIME
    ,@EndCheckInTime DATETIME
    ,@BeginCheckOutTime DATETIME
    ,@EndCheckOutTime DATETIME
    ,@BreakTimeMins INT
    ,@BeginBreakTime DATETIME
    ,@EndBreakTime DATETIME
    ,@LeaveDay DECIMAL(18, 1)
    ,@InOfMachine DATETIME OUT
    ,@OutOfMachine DATETIME OUT
    ,@ExpectedInTime DATETIME OUT
    ,@ExpectedOutTime DATETIME OUT
    ,@ActualLateIn INT OUT
    ,@ActualEarlyOut INT OUT
    ,@TotalWorkHours DECIMAL(18, 2) OUT
AS
/*
Created: 20150729@Quinn.Vo
Task: Calculate time for HR shift.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    DECLARE @DeductEarlyOut INT
        ,@BreakTime INT
        ,@RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
		,@CheckInTemp DATETIME
		,@CheckOutTemp DATETIME

    SET @DeductEarlyOut = 0

    SELECT @InOfMachine = MIN(RecordTime)
    FROM #TimeRecord
    WHERE EmpID = @empId
        AND InOutType = 0
        AND DATEDIFF(d, @WorkDate, RecordTime) = 0

    IF @RequestedCheckinTime IS NOT NULL
    BEGIN
        SET @InOfMachine = @RequestedCheckinTime
    END

    SELECT @OutOfMachine = MIN(RecordTime)
    FROM #TimeRecord
    WHERE EmpID = @empId
        AND InOutType = 1
        -- AND DATEDIFF(d, @WorkDate, RecordTime) = 0
        AND DATEDIFF(MI, DATEADD(HH, 5, @WorkDate), RecordTime) >= 0
        AND DATEDIFF(MI, RecordTime, DATEADD(HH, 5, DATEADD(D, 1, @WorkDate))) >= 0
        AND (
            @InOfMachine IS NULL
            OR RecordTime > DATEADD(MI, 2, @InOfMachine)
            )

    IF @RequestedCheckoutTime IS NOT NULL
    BEGIN
        SET @OutOfMachine = @RequestedCheckoutTime
    END

    SELECT @RequestLeaveTimeBegin = FromTime
        ,@RequestLeaveTimeEnd = ToTime
    FROM #LeaveRequestHalfDay
    WHERE EmpId = @empId
        AND OnDate = @WorkDate

	SELECT @CheckInTemp = @InOfMachine, @CheckOutTemp = @OutOfMachine
    IF (@InOfMachine IS NOT NULL)
    BEGIN
        IF (ISNULL(@LeaveDay, 0) = 0)
        BEGIN
            IF (DATEDIFF(MI, @InOfMachine, @BeginCheckInTime) >= 0) -- <8h
            BEGIN
                SELECT @ExpectedInTime = @BeginCheckInTime, @ExpectedOutTime = @BeginCheckOutTime
            END
            ELSE IF (DATEDIFF(MI, @InOfMachine, @EndCheckOutTime) >= 0) -- 8h-->8h30
            BEGIN
                SELECT @ExpectedInTime = @InOfMachine, @ExpectedOutTime = DATEADD(HH, 9, @InOfMachine)
            END
            ELSE
            BEGIN
                SELECT @ExpectedInTime = @EndCheckInTime, @ExpectedOutTime = @EndCheckOutTime
            END
        END
        ELSE IF (@LeaveDay = 0.5)
        BEGIN
                IF (DATEDIFF(MI, @InOfMachine, @BeginCheckInTime) >= 0) -- <8h
                BEGIN
                    SELECT @ExpectedInTime = @BeginCheckInTime
                        ,@ExpectedOutTime = DATEADD(HH, 4, @BeginCheckInTime)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(HH, 1, @EndCheckInTime)) >= 0 ) -- 8h00->8h30
                BEGIN
                    SELECT @ExpectedInTime = @InOfMachine
                        ,@ExpectedOutTime = DATEADD(HH, 5, @InOfMachine)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(HH, 1, @EndCheckInTime)) >= 0) -- 8h30->9h30
                BEGIN
                    IF (@RequestLeaveTimeBegin < @EndBreakTime)
                    BEGIN
                        SELECT @ExpectedInTime = @EndBreakTime
                            ,@ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                    END
                    ELSE
                    BEGIN
                        SELECT @ExpectedInTime = @EndCheckInTime
                            ,@ExpectedOutTime = DATEADD(HH, 4, @EndCheckInTime)
                    END

                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, @EndBreakTime) >= 0) -- 9h30->13h
                BEGIN
                    SELECT @ExpectedInTime = @EndBreakTime
                        ,@ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(MI, 30, @EndBreakTime)) >= 0) -- 13h-->13h30
                BEGIN
                    SELECT @ExpectedInTime = @InOfMachine
                        ,@ExpectedOutTime =  DATEADD(HH, 4, @InOfMachine)
                END
                ELSE -- > 13h30
                BEGIN
                    SELECT @ExpectedInTime = DATEADD(MI, 30, @EndBreakTime)
                        ,@ExpectedOutTime = @EndCheckOutTime
                END
        END

        IF (
                @OutOfMachine < @EndBreakTime
                AND @ExpectedOutTime > @EndBreakTime
                )
        BEGIN
            SELECT @DeductEarlyOut = CASE 
                    WHEN DATEDIFF(MI, @OutOfMachine, @EndBreakTime) > @BreakTimeMins THEN @BreakTimeMins
                    ELSE DATEDIFF(MI, @OutOfMachine, @EndBreakTime)
                    END
        END

        IF (@OutOfMachine < @ExpectedOutTime)
        BEGIN
            SET @ActualEarlyOut = DATEDIFF(MI, @OutOfMachine, @ExpectedOutTime) - @DeductEarlyOut
        END

        IF (@InOfMachine > @ExpectedINTime)
        BEGIN
            SET @ActualLateIn = DATEDIFF(MI, @ExpectedINTime, @InOfMachine)
        END
    END
    ELSE
    BEGIN
        SELECT @ExpectedInTime = @EndCheckInTime
            ,@ExpectedOutTime = @EndCheckOutTime
    END

	SELECT @InOfMachine = @CheckInTemp, @OutOfMachine = @CheckOutTemp
    
    -- Break time for total hours
    IF (
            @InOfMachine IS NOT NULL
            AND @OutOfMachine IS NOT NULL
		  AND ISNULL(@LeaveDay, 0) <> 1
            )
    BEGIN
        SET @BreakTime = 0

        IF (@InOfMachine < @BeginCheckInTime)
        BEGIN
            SET @BreakTime = DATEDIFF(MI, @InOfMachine, @BeginCheckInTime)
        END

        IF (
                @InOfMachine <= @BeginBreakTime
                AND @OutOfMachine >= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + @BreakTimeMins
        END
        ELSE IF (
                @InOfMachine >= @BeginBreakTime
                AND @OutOfMachine <= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = 1440
        END
        ELSE IF (
                @InOfMachine >= @BeginBreakTime
                AND @InOfMachine <= @EndBreakTime
                AND @OutOfMachine >= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + (DATEDIFF(MI, @InOfMachine, @EndBreakTime))
        END
        ELSE IF (
                @InOfMachine <= @BeginBreakTime
                AND @OutOfMachine >= @BeginBreakTime
                AND @OutOfMachine <= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + (DATEDIFF(MI, @BeginBreakTime, @OutOfMachine))
        END

        SET @TotalWorkHours = ((DATEDIFF(mi, @InOfMachine, @OutOfMachine) - @BreakTime) / 60.0)

        IF (@TotalWorkHours < 0)
        BEGIN
            SET @TotalWorkHours = 0
        END
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM3] 
    @empId INT
    ,@WorkDate DATETIME
    ,@RequestedCheckinTime DATETIME
    ,@RequestedCheckoutTime DATETIME
    ,@BeginCheckInTime DATETIME
    ,@EndCheckInTime DATETIME
    ,@BeginCheckOutTime DATETIME
    ,@EndCheckOutTime DATETIME
    ,@BreakTimeMins INT
    ,@BeginBreakTime DATETIME
    ,@EndBreakTime DATETIME
    ,@LeaveDay DECIMAL(18, 1)
    ,@InOfMachine DATETIME OUT
    ,@OutOfMachine DATETIME OUT
    ,@ExpectedInTime DATETIME OUT
    ,@ExpectedOutTime DATETIME OUT
    ,@ActualLateIn INT OUT
    ,@ActualEarlyOut INT OUT
    ,@TotalWorkHours DECIMAL(18, 2) OUT
AS
/*
Created: 20150729@Quinn.Vo
Task: Calculate time for shift M3.
DB: NexEPA
Original: 

Revisions:  
	- 20250403@Henry: 
Param's Explanation: @ActualLateIn => Always = 0
*/
BEGIN
	
    DECLARE @DeductEarlyOut INT
        ,@BreakTime INT
        ,@RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
        ,@IsUserStandby BIT
	SET @ActualLateIn = 0

    SELECT @DeductEarlyOut = 0, @IsUserStandby = 0
    IF (EXISTS(SELECT 1 FROM Employee WITH (NOLOCK) WHERE GroupId IN (61, 59, 46)))
    BEGIN
        SET @IsUserStandby = 1
    END

    SELECT @InOfMachine = MIN(RecordTime)
    FROM #TimeRecord
    WHERE EmpID = @empId
        AND InOutType = 0
        AND DATEDIFF(d, @WorkDate, RecordTime) = 0

	IF (@InOfMachine IS NULL)
	BEGIN
		SELECT @InOfMachine = MIN(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType IS NULL
			AND DATEDIFF(HH, @WorkDate, DATEADD(MI, 1, RecordTime)) < 14
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0

	END

    IF @RequestedCheckinTime IS NOT NULL
    BEGIN
        SET @InOfMachine = @RequestedCheckinTime
    END
	
    SELECT @OutOfMachine = MIN(RecordTime)
    FROM #TimeRecord
    WHERE EmpID = @empId
        AND InOutType = 1
        -- AND DATEDIFF(d, @WorkDate, RecordTime) = 0
        -- AND DATEDIFF(d, @WorkDate, RecordTime) >= 0
        AND DATEDIFF(MI, DATEADD(HH, 5, @WorkDate), RecordTime) >= 0
        --AND DATEDIFF(MI, RecordTime, DATEADD(HH, 5, DATEADD(D, 1, @WorkDate))) >= 0

        AND ((@IsUserStandby = 0 AND DATEDIFF(MI, RecordTime, DATEADD(HH, 5, DATEADD(D, 1, @WorkDate))) >= 0)
            OR (@IsUserStandby = 1 AND DATEDIFF(MI, RecordTime, DATEADD(HH, 9, DATEADD(D, 1, @WorkDate))) >= 0) -- to 9h
        )
        AND (
            @InOfMachine IS NULL
            OR RecordTime > DATEADD(MI, 2, @InOfMachine)
            )
	
	IF (@OutOfMachine IS NULL)
	BEGIN
		SELECT @OutOfMachine = MIN(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType IS NULL
			AND DATEDIFF(HH, @WorkDate, RecordTime) >= 17
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0
	END

	IF (@OutOfMachine IS NULL)
	BEGIN
		SELECT @OutOfMachine = MAX(RecordTime)
		FROM #TimeRecord
		WHERE EmpID = @empId
			AND InOutType IS NULL
			AND DATEDIFF(HH, @WorkDate, RecordTime) >= 11
			AND DATEDIFF(d, @WorkDate, RecordTime) = 0

		IF (DATEDIFF(HH, @InOfMachine, @OutOfMachine) < 3)
		BEGIN
			SELECT @OutOfMachine = NULL 
		END
	END

    IF @RequestedCheckoutTime IS NOT NULL
    BEGIN
        SET @OutOfMachine = @RequestedCheckoutTime
    END

    SELECT @RequestLeaveTimeBegin = FromTime
                    ,@RequestLeaveTimeEnd = ToTime
                FROM #LeaveRequestHalfDay
                WHERE EmpId = @empId
                    AND OnDate = @WorkDate

    IF (@InOfMachine IS NOT NULL)
    BEGIN
        IF (ISNULL(@LeaveDay, 0) = 0)
        BEGIN
            IF (DATEDIFF(MI, @InOfMachine, @BeginCheckInTime) >= 0) -- <8h
            BEGIN
                SELECT @ExpectedInTime = @BeginCheckInTime
                    ,@ExpectedOutTime = @BeginCheckOutTime
            END
            ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(HH, 2, @BeginCheckInTime)) >= 0) -- 8->10h
            BEGIN
                SELECT @ExpectedInTime = @InOfMachine
                    ,@ExpectedOutTime = DATEADD(HH, 9, @InOfMachine)
            END
            ELSE -- >10h
            BEGIN
                SELECT @ExpectedInTime = @EndCheckInTime
                    ,@ExpectedOutTime = @EndCheckOutTime
            END
        END
        ELSE IF (@LeaveDay = 0.5)
        BEGIN
                IF (DATEDIFF(MI, @InOfMachine, @BeginCheckInTime) >= 0) -- <8h
                BEGIN
                    SELECT @ExpectedInTime = @BeginCheckInTime
                        ,@ExpectedOutTime = DATEADD(HH, 4, @BeginCheckInTime)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, @EndCheckInTime) >= 0) -- 8->10h
                BEGIN
                    SELECT @ExpectedInTime = @InOfMachine
                        ,@ExpectedOutTime = DATEADD(HH, 5, @InOfMachine)
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, @EndBreakTime) >= 0) -- 10h->12h30
                BEGIN

                    IF (@RequestLeaveTimeBegin < @BeginBreakTime)
                    BEGIN
                        SELECT @ExpectedInTime = @EndBreakTime
                            ,@ExpectedOutTime = DATEADD(HH, 4, @EndBreakTime)
                    END
                    ELSE
                    BEGIN
                        SELECT @ExpectedInTime = @EndCheckInTime
                            ,@ExpectedOutTime = DATEADD(HH, 5, @EndCheckInTime)
                    END
                END
                ELSE IF (DATEDIFF(MI, @InOfMachine, DATEADD(HH, 4, @EndCheckInTime)) >= 0) -- 12h30-->14h00
                BEGIN
                    SELECT @ExpectedInTime = @InOfMachine
                        ,@ExpectedOutTime = DATEADD(HH, 4, @InOfMachine)
                END
                ELSE -- >14h00
                BEGIN
                    SELECT @ExpectedInTime = DATEADD(HH, 4, @EndCheckInTime)
                        ,@ExpectedOutTime = DATEADD(HH, 8, @EndCheckInTime)
                END

        END

        IF (
                @OutOfMachine < @EndBreakTime
                AND @ExpectedOutTime > @EndBreakTime
                )
        BEGIN
            SELECT @DeductEarlyOut = CASE 
                    WHEN DATEDIFF(MI, @OutOfMachine, @EndBreakTime) > @BreakTimeMins THEN @BreakTimeMins
                    ELSE DATEDIFF(MI, @OutOfMachine, @EndBreakTime)
                    END
        END

        IF (@OutOfMachine < @ExpectedOutTime)
        BEGIN
            SET @ActualEarlyOut = DATEDIFF(MI, @OutOfMachine, @ExpectedOutTime) - @DeductEarlyOut
        END

        --IF (@InOfMachine > @ExpectedINTime)
        --BEGIN
        --    SET @ActualLateIn = DATEDIFF(MI, @ExpectedINTime, @InOfMachine)
        --END
    END
    ELSE
    BEGIN
        SELECT @ExpectedInTime = @EndCheckInTime
            ,@ExpectedOutTime = @EndCheckOutTime
    END

    -- Break time for total hours
    IF (
            @InOfMachine IS NOT NULL
            AND @OutOfMachine IS NOT NULL
		  AND (ISNULL(@LeaveDay, 0) <> 1)
            )
    BEGIN
        SET @BreakTime = 0

        IF (@InOfMachine < @BeginCheckInTime)
        BEGIN
            SET @BreakTime = DATEDIFF(MI, @InOfMachine, @BeginCheckInTime)
        END

        IF (
                @InOfMachine <= @BeginBreakTime
                AND @OutOfMachine >= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + @BreakTimeMins
        END
        ELSE IF (
                @InOfMachine >= @BeginBreakTime
                AND @OutOfMachine <= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = 1440
        END
        ELSE IF (
                @InOfMachine >= @BeginBreakTime
                AND @InOfMachine <= @EndBreakTime
                AND @OutOfMachine >= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + (DATEDIFF(MI, @InOfMachine, @EndBreakTime))
        END
        ELSE IF (
                @InOfMachine <= @BeginBreakTime
                AND @OutOfMachine >= @BeginBreakTime
                AND @OutOfMachine <= @EndBreakTime
                )
        BEGIN
            SET @BreakTime = @BreakTime + (DATEDIFF(MI, @BeginBreakTime, @OutOfMachine))
        END
        
        SET @TotalWorkHours = ((DATEDIFF(mi, @InOfMachine, @OutOfMachine) - @BreakTime) / 60.0)

        IF (@TotalWorkHours < 0)
        BEGIN
            SET @TotalWorkHours = 0
        END
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ProcessingData] 
AS
BEGIN

DECLARE @i INT
		,@TempEmpId INT
		,@TempWorkDate DATE
		,@TempShiftId INT
		,@fromDate DATE
		,@toDate DATE
		,@CurrentDateTime DATETIME
		,@TempResignedDate DATETIME

SELECT @CurrentDateTime = dbo.fnc_GetClientDateTime()


DECLARE @tblEmployee TABLE (
		EmpId INT
		,WorkDate DATE
		,ResignedDate DATETIME
		)
SELECT @fromDate = MIN(tmpAtt.WorkDate)
		,@toDate = MAX(tmpAtt.WorkDate)
	FROM dbo.tmpEmployeeAttendance AS tmpAtt WITH (NOLOCK)

	INSERT INTO @tblEmployee (
		EmpId
		,WorkDate
		)
	SELECT TOP 100 EmpId
		,WorkDate
	FROM dbo.tmpEmployeeAttendance AS tmpAtt WITH (NOLOCK)
	select * from @tblEmployee
	DELETE tmp
	FROM dbo.tmpEmployeeAttendance AS tmp
		INNER JOIN @tblEmployee AS e ON tmp.EmpId = e.EmpId
		AND tmp.WorkDate = e.WorkDate

IF OBJECT_ID('#TimeRecord') IS NOT NULL
BEGIN
    DROP TABLE #TimeRecord
END 
CREATE TABLE #TimeRecord (Id INT, EmpID INT, RecordTime DATETIME, InOutType TINYINT NULL)

-- Get all in/out data into temp table to calculate
INSERT INTO #TimeRecord    (Id, EmpID, RecordTime, InOutType)
SELECT tr.Id, EmpID, RecordTime, InOutType
FROM dbo.TimeRecord AS tr WITH (NOLOCK)
LEFT JOIN dbo.TimeRecordExt AS te WITH (NOLOCK) ON tr.Id = te.ID
	WHERE RecordTime BETWEEN @fromDate
			AND DATEADD(d, 2, @toDate)
		AND EXISTS (
			SELECT 1
			FROM @tblEmployee
			WHERE EmpId = tr.EmpId
			)
	AND te.ID IS NULL OR (te.[Status] = 1 OR te.[Status] = 2 OR te.[Status] = 4)


IF OBJECT_ID('#LeaveRequestHalfDay') IS NOT NULL
BEGIN
    DROP TABLE #LeaveRequestHalfDay
END 
CREATE TABLE #LeaveRequestHalfDay (EmpID INT, OnDate DATETIME, FromTime DATETIME, ToTime DATETIME)

-- Get all leave request half day
INSERT INTO #LeaveRequestHalfDay (EmpID, OnDate, FromTime, ToTime)
SELECT EmpId, CONVERT(VARCHAR(10), FromDate, 111), FromDate, ToDate
FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
WHERE [Status] IN (1, 2)
    AND TotalDays = 0.5
    AND (EXISTS (SELECT 1 FROM @tblEmployee WHERE EmpId = wt.EmpId))
    AND DATEDIFF(d, @fromDate, FromDate) >= 0
    AND DATEDIFF(d, FromDate, @toDate) >= 0

-- Get all calendars of employees into temp table
IF OBJECT_ID('#EmpCalendar') IS NOT NULL
BEGIN
    DROP TABLE #EmpCalendar
END 
CREATE TABLE #EmpCalendar (
        Id INT PRIMARY KEY IDENTITY(1,1), 
        EmpId INT,
        WorkDate DATETIME ,
        ShiftId INT,
        IsHoliday BIT,
        IsOffDay BIT,
        IsLeaveDay BIT,
        LeaveTypeId INT NULL,
		LeaveTypeId2 INT NULL,
        LateIn INT,
        EarlyOut INT,
        RequestedCheckInTime DATETIME ,
        RequestedCheckOutTime DATETIME ,
        BeginCheckInTime DATETIME,
        EndCheckInTime DATETIME,
        BeginCheckOutTime DATETIME,
        EndCheckOutTime DATETIME,
        BreakTimeMins INT,
        BeginBreakTime DATETIME,
        EndBreakTime DATETIME,
        LeaveDay DECIMAL(18,1),
        TimeForPostnatal INT)

INSERT INTO #EmpCalendar (
        EmpId,
        WorkDate,
        ShiftId,
        IsHoliday,
        IsOffDay,
        IsLeaveDay,
        LeaveTypeId,
		LeaveTypeId2,
        LateIn,
        EarlyOut,        
        RequestedCheckInTime,
        RequestedCheckOutTime,
        BeginCheckInTime,
        EndCheckInTime,
        BeginCheckOutTime,
        EndCheckOutTime,
        BreakTimeMins,
        BeginBreakTime,
        EndBreakTime,
        LeaveDay,
        TimeForPostnatal)
SELECT     
        ec.EmpId,
        WorkDate,
        ec.ShiftId,
        IsHoliday,
        IsOffDay,
        IsLeaveDay,
        LeaveTypeId,
		LeaveTypeId2,
        LateIn,
        EarlyOut,        
        RequestedCheckinTime,
        RequestedCheckoutTime,
        CASE WHEN IsInAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
            ELSE CAST(WorkDate + ' ' + s.BeginCheckInTime AS DATETIME)
            END AS [BeginCheckInTime],
        CASE WHEN IsInAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
            ELSE CAST(WorkDate + ' ' + s.EndCheckInTime AS DATETIME)
            END AS [EndCheckInTime],

        CASE WHEN IsOutAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.BeginCheckOutTime )
            ELSE CAST(WorkDate + ' ' + s.BeginCheckOutTime AS DATETIME)
            END AS [BeginCheckOutTime],

        CASE WHEN IsOutAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckOutTime )
            ELSE CAST(WorkDate + ' ' + s.EndCheckOutTime AS DATETIME)
            END AS [EndCheckOutTime],
        BreakTimeMins,
        CAST(WorkDate + ' ' + s.BeginBreakTime AS DATETIME),
        CAST(WorkDate + ' ' + s.EndBreakTime AS DATETIME),
        LeaveDay,
        CASE WHEN pos.EmpId IS NULL THEN 0 ELSE (CASE WHEN (LeaveDay = 0.5 AND LeaveTypeId <> 9) THEN 30 ELSE pos.[Minutes] END) END
FROM dbo.EmployeeCalendar ec WITH (NOLOCK)
LEFT JOIN dbo.Shift s WITH (NOLOCK)ON s.Id = ec.ShiftId
LEFT JOIN dbo.EmployeePostnatal pos WITH (NOLOCK) ON ec.EmpId = pos.EmpId
AND ec.WorkDate BETWEEN pos.FromDate AND pos.ToDate
WHERE (EXISTS (SELECT 1 FROM @tblEmployee WHERE EmpId = ec.EmpId AND WorkDate = ec.WorkDate))
ORDER BY ec.EmpId, ec.WorkDate

SET @i = 1
IF EXISTS (
			SELECT 1
			FROM #EmpCalendar
			)
	BEGIN
		SET @i = 1

		WHILE EXISTS (
				SELECT 1
				FROM #EmpCalendar
				WHERE Id = @i
				)
		BEGIN
			SELECT @TempEmpId = EmpId
				,@TempWorkDate = WorkDate
				,@TempShiftId = ShiftId
			FROM #EmpCalendar
			WHERE Id = @i

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId_new @TempEmpId
				,@TempWorkDate
				,@TempWorkDate
				,1

			SET @i = @i + 1
		END
	END

DROP TABLE #TimeRecord
DROP TABLE #EmpCalendar
DROP TABLE #LeaveRequestHalfDay
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_ReminderLateInEarlyOut] @EmpId INT
AS
/*
Created: 20180919@Quinn.Vo
Task: Get Employees must be created leave half day request.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
	SET @ToDate = dbo.fnc_GetClientDateTime()
	SET @FromDate = DATEADD(MM, -1, @ToDate)

    SELECT att.AttendanceDate
		,att.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.Id
	WHERE att.EmpId = @EmpId
	AND att.AttendanceDate BETWEEN @FromDate AND @ToDate
	AND (ISNULL(att.LateIn, 0) > 60 OR ISNULL(att.EarlyOut, 0) > 60)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_TotalWorkingHoursToday]
    @empid INT,
    @TotalWorkHoursToday DECIMAL(18, 2) OUT
AS
BEGIN
DECLARE @i INT, @TempEmpId INT, @CurrentDateTime DATETIME, @TempResignedDate DATETIME

SELECT @CurrentDateTime = dbo.fnc_GetClientDateTime()

IF OBJECT_ID('#Employee') IS NOT NULL
BEGIN
    DROP TABLE #Employee
END 
CREATE TABLE #Employee (Id INT PRIMARY KEY IDENTITY(1,1), EmpId INT, ResignedDate DATETIME)

IF OBJECT_ID('#TimeRecord') IS NOT NULL
BEGIN
    DROP TABLE #TimeRecord
END 
CREATE TABLE #TimeRecord (Id INT, EmpID INT, RecordTime DATETIME, InOutType TINYINT)

-- Get all in/out data into temp table to calculate
INSERT INTO #TimeRecord    (Id, EmpID, RecordTime, InOutType)
SELECT Id, EmpID, RecordTime, InOutType 
FROM dbo.TimeRecord WITH (NOLOCK) 
WHERE (@empId IS NULL OR EmpID = @empId)
    AND DATEDIFF(d, @CurrentDateTime, RecordTime) >= 0
    AND DATEDIFF(d, @CurrentDateTime, RecordTime) <= 1
---- insert virtual checkout time (now)
INSERT INTO #TimeRecord (Id, EmpID, RecordTime, InOutType)
VALUES(null, @empid, dbo.fnc_GetClientDateTime(), 1)

IF OBJECT_ID('#LeaveRequestHalfDay') IS NOT NULL
BEGIN
    DROP TABLE #LeaveRequestHalfDay
END 
CREATE TABLE #LeaveRequestHalfDay (EmpID INT, OnDate DATETIME, FromTime DATETIME, ToTime DATETIME)

-- Get all leave request half day
INSERT INTO #LeaveRequestHalfDay (EmpID, OnDate, FromTime, ToTime)
SELECT EmpId, CONVERT(VARCHAR(10), FromDate, 111), FromDate, ToDate
FROM dbo.WorkingTimeRequest WITH (NOLOCK)
WHERE [Status] IN (1, 2)
    AND TotalDays = 0.5
    AND (@empId IS NULL OR EmpId = @empId)
    AND DATEDIFF(d, dbo.fnc_GetClientDateTime(), FromDate) >= 0

-- Get all calendars of employees into temp table
IF OBJECT_ID('#EmpCalendar') IS NOT NULL
BEGIN
    DROP TABLE #EmpCalendar
END 
CREATE TABLE #EmpCalendar (
        Id INT PRIMARY KEY IDENTITY(1,1), 
        EmpId INT,
        WorkDate DATETIME ,
        ShiftId INT,
        IsHoliday BIT,
        IsOffDay BIT,
        IsLeaveDay BIT,
        LeaveTypeId INT NULL,
        LateIn INT,
        EarlyOut INT,
        RequestedCheckInTime DATETIME ,
        RequestedCheckOutTime DATETIME ,
        BeginCheckInTime DATETIME,
        EndCheckInTime DATETIME,
        BeginCheckOutTime DATETIME,
        EndCheckOutTime DATETIME,
        BreakTimeMins INT,
        BeginBreakTime DATETIME,
        EndBreakTime DATETIME,
        LeaveDay DECIMAL(18,1),
        TimeForPostnatal INT)

INSERT INTO #EmpCalendar (
        EmpId,
        WorkDate,
        ShiftId,
        IsHoliday,
        IsOffDay,
        IsLeaveDay,
        LeaveTypeId,
        LateIn,
        EarlyOut,        
        RequestedCheckInTime,
        RequestedCheckOutTime,
        BeginCheckInTime,
        EndCheckInTime,
        BeginCheckOutTime,
        EndCheckOutTime,
        BreakTimeMins,
        BeginBreakTime,
        EndBreakTime,
        LeaveDay,
        TimeForPostnatal)
SELECT     
        ec.EmpId,
        WorkDate,
        ec.ShiftId,
        IsHoliday,
        IsOffDay,
        IsLeaveDay,
        LeaveTypeId,
        LateIn,
        EarlyOut,        
        RequestedCheckinTime,
        RequestedCheckoutTime,
        CASE WHEN IsInAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
            ELSE CAST(WorkDate + ' ' + s.BeginCheckInTime AS DATETIME)
            END AS [BeginCheckInTime],
        CASE WHEN IsInAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
            ELSE CAST(WorkDate + ' ' + s.EndCheckInTime AS DATETIME)
            END AS [EndCheckInTime],

        CASE WHEN IsOutAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.BeginCheckOutTime )
            ELSE CAST(WorkDate + ' ' + s.BeginCheckOutTime AS DATETIME)
            END AS [BeginCheckOutTime],

        CASE WHEN IsOutAfterDay = 1 -- if the range is cross-date
            THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckOutTime )
            ELSE CAST(WorkDate + ' ' + s.EndCheckOutTime AS DATETIME)
            END AS [EndCheckOutTime],
        BreakTimeMins,
        CAST(WorkDate + ' ' + s.BeginBreakTime AS DATETIME),
        CAST(WorkDate + ' ' + s.EndBreakTime AS DATETIME),
        LeaveDay,
        CASE WHEN pos.EmpId IS NULL THEN 0 ELSE (CASE WHEN (LeaveDay = 0.5 AND LeaveTypeId <> 9) THEN (pos.[Minutes]/2.0) ELSE pos.[Minutes] END) END
FROM dbo.EmployeeCalendar ec WITH (NOLOCK)
LEFT JOIN dbo.Shift s WITH (NOLOCK)ON s.Id = ec.ShiftId
LEFT JOIN dbo.EmployeePostnatal pos WITH (NOLOCK) ON ec.EmpId = pos.EmpId
AND ec.WorkDate BETWEEN pos.FromDate AND pos.ToDate
WHERE (@EmpId IS NULL OR ec.EmpId = @EmpId)
AND ec.WorkDate = convert(date, dbo.fnc_GetClientDateTime())
ORDER BY ec.EmpId, ec.WorkDate

IF @empId IS NOT NULL
BEGIN
    INSERT INTO #Employee (EmpId, ResignedDate)
    SELECT Id, ResignedDate FROM dbo.Employee WITH (NOLOCK) WHERE Id = @empid
END
BEGIN
    SELECT @TempEmpId = EmpId, @TempResignedDate = ResignedDate FROM #Employee WHERE Id = @empId    
    EXEC [EmployeeAttendance_TotalWorkingHoursToday_ForEmpId] @empId, @TotalWorkHoursToday = @TotalWorkHoursToday OUTPUT    

END

DROP TABLE #TimeRecord
DROP TABLE #EmpCalendar
DROP TABLE #LeaveRequestHalfDay
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeAttendance_TotalWorkingHoursToday_ForEmpId]
    @empId INT,
    @TotalWorkHoursToday DECIMAL(18, 2) OUT
AS
BEGIN
    DECLARE @RangeToScanTime INT
        ,@IsFlexibleTime BIT
        ,@DeductEarlyOut INT
        ,@BreakTime INT
        ,@i INT
        ,@WorkDate DATETIME
        ,@ShiftId INT
        ,@IsHoliday BIT
        ,@isOffDay BIT
        ,@IsLeaveDay BIT
        ,@LeaveTypeId INT
        ,@LateIn INT
        ,@EarlyOut INT
        ,@RequestedCheckinTime DATETIME
        ,@RequestedCheckoutTime DATETIME
        ,@BeginCheckInTime DATETIME
        ,@EndCheckInTime DATETIME
        ,@BeginCheckOutTime DATETIME
        ,@EndCheckOutTime DATETIME
        ,@InOfMachine DATETIME
        ,@OutOfMachine DATETIME
        ,@ExpectedInTime DATETIME
        ,@ExpectedOutTime DATETIME
        ,@Absence BIT
        ,@ActualLateIn INT
        ,@ActualEarlyOut INT
        ,@TotalWorkHours DECIMAL(18, 2)
        ,@BreakTimeMins INT
        ,@BeginBreakTime DATETIME
        ,@EndBreakTime DATETIME
        ,@LeaveDay DECIMAL(18, 1)
        ,@RequestLeaveTimeBegin DATETIME
        ,@RequestLeaveTimeEnd DATETIME
        ,@adjustLateIn INT
        ,@adjustEarlyOut INT
        ,@TimeForPostnatal INT

    SET @i = 0
    SET @WorkDate = CONVERT(VARCHAR(10), dbo.fnc_GetClientDateTime(), 111)

    IF (DATEDIFF(d, @WorkDate, dbo.fnc_GetClientDateTime()) >= 0)
    BEGIN                
        SELECT @Absence = 0
            ,@InOfMachine = NULL
            ,@OutOfMachine = NULL
            ,@ExpectedOutTime = NULL
            ,@ExpectedInTime = NULL
            ,@ExpectedOutTime = NULL
            ,@ActualLateIn = 0
            ,@ActualEarlyOut = 0
            ,@RequestedCheckinTime = NULL
            ,@RequestedCheckoutTime = NULL
            ,@TotalWorkHours = NULL
            ,@BreakTimeMins = 0
            ,@ShiftId = NULL
            ,@LeaveTypeId = NULL
            ,@RequestLeaveTimeBegin = NULL
            ,@RequestLeaveTimeEnd = NULL
            ,@adjustLateIn = 0
            ,@adjustEarlyOut = 0
            ,@DeductEarlyOut = 0
            ,@BeginBreakTime = NULL
            ,@EndBreakTime = NULL
            ,@TimeForPostnatal = 0

        SELECT @WorkDate = WorkDate
            ,@ShiftId = ShiftId
            ,@IsHoliday = IsHoliday
            ,@isOffDay = IsOffDay
            ,@IsLeaveDay = IsLeaveDay
            ,@LeaveTypeId = LeaveTypeId
            ,@LateIn = ISNULL(LateIn, 0)
            ,@EarlyOut = ISNULL(EarlyOut, 0)
            ,@RequestedCheckinTime = RequestedCheckinTime
            ,@RequestedCheckoutTime = RequestedCheckoutTime
            ,@BeginCheckInTime = BeginCheckInTime
            ,@EndCheckInTime = EndCheckInTime
            ,@BeginCheckOutTime = BeginCheckOutTime
            ,@EndCheckOutTime = EndCheckOutTime
            ,@BreakTimeMins = BreakTimeMins
            ,@BeginBreakTime = BeginBreakTime
            ,@EndBreakTime = EndBreakTime
            ,@LeaveDay = LeaveDay
            ,@TimeForPostnatal = TimeForPostnatal
        FROM #EmpCalendar
        WHERE EmpId = @empId
            AND WorkDate = @WorkDate
        
        -- If Has Shift                    
        IF @ShiftId IS NOT NULL
        BEGIN
            IF (@LeaveTypeId = 9 OR @LeaveTypeId = 32)
			BEGIN
				EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShift_BusinessTrip] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
			END
			ELSE IF (@ShiftId = 10) -- shift M1
            BEGIN
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM1] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
            ELSE IF (@ShiftId = 11) -- shift M2
            BEGIN
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM2] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
            ELSE IF (@ShiftId = 12) -- shift M3
            BEGIN
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShiftM3] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
            ELSE -- shift # M1, M2, M3
            BEGIN
                EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForShift] @empId = @empId
                    ,@WorkDate = @WorkDate
                    ,@IsLeaveDay = @IsLeaveDay
                    ,@RequestedCheckinTime = @RequestedCheckinTime
                    ,@RequestedCheckoutTime = @RequestedCheckoutTime
                    ,@BeginCheckInTime = @BeginCheckInTime
                    ,@EndCheckInTime = @EndCheckInTime
                    ,@BeginCheckOutTime = @BeginCheckOutTime
                    ,@EndCheckOutTime = @EndCheckOutTime
                    ,@BreakTimeMins = @BreakTimeMins
                    ,@BeginBreakTime = @BeginBreakTime
                    ,@EndBreakTime = @EndBreakTime
                    ,@LeaveDay = @LeaveDay
                    ,@InOfMachine = @InOfMachine OUT
                    ,@OutOfMachine = @OutOfMachine OUT
                    ,@ExpectedInTime = @ExpectedInTime OUT
                    ,@ExpectedOutTime = @ExpectedOutTime OUT
                    ,@ActualLateIn = @ActualLateIn OUT
                    ,@ActualEarlyOut = @ActualEarlyOut OUT
                    ,@TotalWorkHours = @TotalWorkHours OUT
            END
        END
        ELSE
        BEGIN -- No shift in attendance date 
            EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForNoShift] @empId = @empId
                ,@WorkDate = @WorkDate
                ,@ShiftId = @ShiftId
                ,@IsLeaveDay = @IsLeaveDay
                ,@LeaveTypeId = @LeaveTypeId
                ,@LateIn = @LateIn
                ,@EarlyOut = @EarlyOut
                ,@RequestedCheckinTime = @RequestedCheckinTime
                ,@RequestedCheckoutTime = @RequestedCheckoutTime
                ,@BeginCheckInTime = @BeginCheckInTime
                ,@EndCheckInTime = @EndCheckInTime
                ,@BeginCheckOutTime = @BeginCheckOutTime
                ,@EndCheckOutTime = @EndCheckOutTime
                ,@BreakTimeMins = @BreakTimeMins
                ,@BeginBreakTime = @BeginBreakTime
                ,@EndBreakTime = @EndBreakTime
                ,@LeaveDay = @LeaveDay
                ,@TimeForPostnatal = @TimeForPostnatal
                ,@InOfMachine = @InOfMachine OUT
                ,@OutOfMachine = @OutOfMachine OUT
                ,@ExpectedInTime = @ExpectedInTime OUT
                ,@ExpectedOutTime = @ExpectedOutTime OUT
                ,@ActualLateIn = @ActualLateIn OUT
                ,@ActualEarlyOut = @ActualEarlyOut OUT
                ,@TotalWorkHours = @TotalWorkHours OUT
        END

        -- hau san 7h
        IF(@TimeForPostnatal > 0 AND @ActualLateIn > 0)
        BEGIN
            IF (@TimeForPostnatal > @ActualLateIn)
            BEGIN
                SET @TimeForPostnatal = @TimeForPostnatal - @ActualLateIn
                SET @ActualLateIn = 0
            END
            ELSE
            BEGIN
                SET @ActualLateIn = @ActualLateIn - @TimeForPostnatal
                SET @TimeForPostnatal = 0
            END
        END

        IF(@TimeForPostnatal > 0 AND @ActualEarlyOut > 0)
        BEGIN
            IF (@TimeForPostnatal > @ActualEarlyOut)
            BEGIN
                SET @TimeForPostnatal = @TimeForPostnatal - @ActualEarlyOut
                SET @ActualEarlyOut = 0
            END
            ELSE
            BEGIN
                SET @ActualEarlyOut = @ActualEarlyOut - @TimeForPostnatal
                SET @TimeForPostnatal = 0
            END
        END

        IF (
                @isOffDay = 1
                OR @IsHoliday = 1
                OR (
                    @IsLeaveDay = 1
                    AND @LeaveDay = 1
                    )
                )
        BEGIN
            SET @Absence = 1
        END
        ELSE IF (
                @InOfMachine IS NULL
                AND @OutOfMachine IS NULL
                )
        BEGIN
            SET @Absence = 1
        END
        --Code here--                

		SET @TotalWorkHoursToday = @TotalWorkHours
    END
END    
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_CheckExistedCalendar]
	@empid int, 
	@fromdate datetime, 
	@todate datetime
AS	
/*
Created: 2012/04/20@Dan
Task: approve request form (main SP)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

declare	@maxDate datetime, @minDate datetime, @result bit
set @result=0

select @maxDate=max(workdate),@minDate = min(workdate)
from dbo.EmployeeCalendar
where empid=@empid
	and workdate between @fromdate and @todate	

if (@fromdate = @minDate and @todate = @maxDate)
	set @result = 1

select @result
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_Get] -- '2022-01-04 11:20:25.000'
    @FromDate DATETIME = NULL
AS
/*
Created: 20141002@Quinn.Vo
Task: Get employee calendar from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT 
      [WorkDate]
      ,[EmpId] AS UserId
      ,CASE WHEN ISNULL(IsWorkFromHome, 0) = 1 THEN NULL ELSE [ShiftId] END [ShiftId]
	  --,[ShiftId]
      ,[IsHoliday]
      ,CAST(CASE WHEN [IsOffDay] = 1 OR ISNULL(IsWorkFromHome, 0) = 1 THEN 1 ELSE 0 END AS BIT) AS [IsOffDay]
       ,[IsLeaveDay]
      ,[LeaveTypeId]
      ,[LeaveDay]
      ,cl.ModifiedDate
    FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
    WHERE @FromDate IS NULL
    OR cl.ModifiedDate > @FromDate
    ORDER BY cl.ModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetChangedShift]
	@reportDate datetime,
	@employeeId int = null,
	@groupid int = null
AS
/*
Created: 2012/06/20@Dan
Task: get changed shift in a month (it's used for coloring the day in report)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

SET NOCOUNT ON

DECLARE @Days varchar(1000), @cols varchar(2000), @totalDays int, 
		@fromDate datetime, @toDate datetime

-- first day of month
set @fromDate= cast(YEAR(@reportDate) as varchar(4)) + '/' + cast(MONTH(@reportDate) as varchar(2)) + '/01'

-- last day of month
set @ToDate = dateadd(d,-1, dateadd(m,1,@fromdate))

SELECT e.ID, e.NickName, e.FullName, DAY(workdate) as Day, s.Code 
FROM dbo.EmployeeCalendar c with (nolock)
	join employee e with (nolock) on e.id=c.empid
	left join Shift s on s.id= c.shiftid			
WHERE WorkDate between @fromDate and @toDate
	and (@employeeId is null or empid=@employeeId)
	and (@groupid  is null or e.groupid=@groupid)
	and ChangeShiftId is not null
	and IsHoliday=0 and IsOffDay=0 and IsLeaveDay=0
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetCurrentUserWorkedDates]
    @UserId INT
    ,@FromDate DATETIME
    ,@ToDate DATETIME
AS
/*
	Created: 20180614@Oliver.Dang
	Task: Get employee working dates by date range.
	DB: NexHRM
	Original: 

	Revisions:  
		- 20250327@Henry: Update MGT Code -> MGT/COR code
		- 20250506@Henry: Add HR-ADM group code
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @IsNotShow BIT = 0

	IF(
		EXISTS (
			SELECT 1 FROM 
				dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
			WHERE e.Id = @UserId
				AND 
					(e.Roles LIKE '%2%'
						OR e.Roles LIKE '%3%'
						OR e.Roles LIKE '%4%'
						OR g.Code IN ('HR', 'EXP','FA','HR-ADM','PM', 'MGT', 'COR', 'ITNW')
					)
		)
	)
	BEGIN
		SET @IsNotShow = 1
	END

	CREATE TABLE #EmployeeCalendar2HalfDay
	(
		WorkDate DATE
	)

	INSERT INTO #EmployeeCalendar2HalfDay(WorkDate)
	SELECT CAST(rq.FromDate AS DATE)
	FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
	WHERE rq.EmpId = @UserId
		AND rq.[Status] IN (1, 2)
		AND rq.TotalDays = 0.5
	GROUP BY rq.EmpId
		,CAST(rq.FromDate AS DATE)
	HAVING SUM(TotalDays) = 1

    SELECT cl.WorkDate
    FROM  dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	LEFT JOIN dbo.WorkingTimeRequest AS rq WITH (NOLOCK) ON cl.EmpId = rq.EmpId
		AND cl.WorkDate BETWEEN rq.FromDate AND rq.ToDate
		AND rq.[Status] = 1
		AND rq.TotalDays >= 1
	LEFT JOIN #EmployeeCalendar2HalfDay AS hd ON cl.WorkDate = hd.WorkDate
    WHERE cl.EmpId = @UserId
		AND (cl.WorkDate between @FromDate and @ToDate)
		AND (cl.IsOffDay <> 1 AND ISNULL(cl.LeaveDay, 0) < 1 AND cl.IsHoliday <> 1)
		AND rq.Id IS NULL
		AND hd.WorkDate IS NULL
		AND @IsNotShow = 0

	DROP TABLE #EmployeeCalendar2HalfDay
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetHalfDayOff]
	@reportDate datetime,
	@employeeId int = null,
	@groupid int = null
AS
/*
Created: 2012/06/20@Dan
Task: get changed shift in a month (it's used for coloring the day in report)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

SET NOCOUNT ON

DECLARE @Days varchar(1000), @cols varchar(2000), @totalDays int, 
		@fromDate datetime, @toDate datetime

-- first day of month
set @fromDate= cast(YEAR(@reportDate) as varchar(4)) + '/' + cast(MONTH(@reportDate) as varchar(2)) + '/01'

-- last day of month
set @ToDate = dateadd(d,-1, dateadd(m,1,@fromdate))

SELECT e.ID, e.NickName, e.FullName, DAY(workdate) as Day, s.Code 
FROM dbo.EmployeeCalendar c with (nolock)
	join employee e with (nolock) on e.id=c.empid
	left join Shift s on s.id= c.shiftid			
WHERE 	WorkDate between @fromDate and @toDate
	and (@employeeId is null or empid=@employeeId)
	and (@groupid  is null or e.groupid=@groupid)
	and IsHoliday =0  and IsOffDay=0 and	IsLeaveDay = 1 
	and LeaveDay = 0.5
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetLeaveSchedule]
    @FromDate DATETIME
    ,@ToDate DATETIME
    ,@EmployeeId INT = NULL
    ,@GroupId INT = NULL
    ,@DepartmentId INT = NULL
AS
/*
Created: 20180515@Oliver.Dang
Task: Get employee leave schedule by date.
DB: NexEPA
Original: 

Revisions:  
    - 20250515@Henry.Dinh: Add department
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT cl.WorkDate
       ,(CASE WHEN cl.IsLeaveDay = 1 AND cl.LeaveDay = 0.5 THEN 1 ELSE 0 END) AS IsHalfDayLeave
	   ,(CASE WHEN IsOffDay = 1 OR cl.IsHoliday = 1 THEN 0 ELSE cl.LeaveDay END) AS LeaveDay
	   ,cl.IsLeaveDay AS IsLeaveDaye
       ,(CASE 
            WHEN cl.isHoliday = 1 THEN 'PH'
            WHEN isOffday = 1 THEN 'OFF'
            WHEN LeaveDay = 0.5 THEN '½'
			WHEN LeaveDay = 1 THEN 'x'
            ELSE ''
            END) AS LeaveType
         ,cl.EmpId
        ,e.NickName
        ,e.FullName
        ,d.Name AS DepartmentName
        ,e.GroupId AS TeamId
        ,g.Code AS TeamCode
        ,g.Name AS TeamName
    FROM dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON e.Id = cl.EmpId
    INNER JOIN dbo.EmployeeGroup AS g ON g.Id = e.GroupId
    INNER JOIN dbo.Department AS d ON d.DepartmentId = g.DepartmentId
     LEFT JOIN dbo.[Shift] AS s ON s.Id = cl.ShiftId
    WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND cl.WorkDate BETWEEN @fromDate
            AND @toDate
        AND (
            @EmployeeId IS NULL
            OR e.Id = @EmployeeId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
        AND (
            @DepartmentId IS NULL
            OR g.DepartmentId = @DepartmentId
            )
    ORDER BY d.Name
        ,g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate
        ,cl.WorkDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetRangeDate] 
    @EmpId INT
    ,@fromDate DATETIME
    ,@toDate DATETIME
	,@RequestId BIGINT = NULL
AS
/*
Created: 2018/12/10@Quinn.Vo
Task: Get number of days when create/edit leave request.
DB: NexHR
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @TotalDay NUMERIC(5,1)

	IF (@RequestId IS NULL)
	BEGIN
		SELECT @TotalDay = COUNT(1)
		FROM dbo.EmployeeCalendar WITH (NOLOCK)
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN @fromDate
				AND @toDate
			AND IsHoliday = 0
			AND IsOffDay = 0
			AND IsLeaveDay = 0
	END
	ELSE
	BEGIN
		SELECT @TotalDay = COUNT(1)
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE EmpID = @EmpId
				AND WorkDate BETWEEN @fromDate
					AND @toDate
				AND IsHoliday = 0
				AND IsOffDay = 0
	END

	SELECT @TotalDay AS TotalDay
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetShiftByDate]
	@employeeId int,
	@date datetime
AS
BEGIN
	SET NOCOUNT ON;

	select * from Shift
	where Id = (select top(1) ShiftId from EmployeeCalendar
				where	EmpId = @employeeId and
						DATEDIFF(day,WorkDate,@date)=0)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetUnavailRequestDate] -- 245, '6/1/2014', '9/30/2014'
	@EmpId INT
	,@fromDate DATETIME
	,@toDate DATETIME
	,@requestId BIGINT = NULL
AS
/*
Created: 2018/12/10@Quinn.Vo
Task: Get unavailable date when create/edit request.
DB: NexHR
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @MaxDate DATETIME
		,@MinDate DATETIME
		,@fromDate_LC DATETIME
		,@toDate_LC DATETIME

	CREATE TABLE #tmpCalendar (WorkDate DATETIME)

	SELECT @fromDate = CONVERT(NVARCHAR(20), @fromDate, 111)
		,@toDate = CONVERT(NVARCHAR(20), @toDate, 111)

	SELECT @fromDate_LC = @fromDate
		,@toDate_LC = @toDate

	WHILE (@fromDate_LC <= @toDate_LC)
	BEGIN
		INSERT INTO #tmpCalendar
		VALUES (@fromDate_LC)

		SET @fromDate_LC = DATEADD(DD, 1, @fromDate_LC)
	END

	DELETE #tmpCalendar
	WHERE WorkDate IN (
			SELECT WorkDate
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE (
					WorkDate BETWEEN @fromDate
						AND @toDate
					)
				AND EmpID = @EmpId
				AND NOT (
					IsOffDay = 1
					OR IsHoliday = 1
					OR ShiftId IS NULL
					OR (
						IsLeaveDay = 1
						AND LeaveDay = 1
						)
					)
			)

	-- AND NOT (((IsHoliday = 1 OR IsOffDay = 1) AND ShiftId IS NULL) OR (IsLeaveDay = 1 and LeaveDay=1))
	IF (@requestId IS NOT NULL)
	BEGIN
		DELETE #tmpCalendar
		WHERE WorkDate IN (
				SELECT cl.WorkDate
				FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
					INNER JOIN dbo.WorkingTimeRequest AS rq WITH (NOLOCK) ON cl.EmpId = rq.EmpId
					AND DATEDIFF(DD, rq.FromDate, cl.WorkDate) >= 0
					AND DATEDIFF(DD, cl.WorkDate, rq.ToDate) >= 0
					AND rq.TotalDays > 0.5
					AND rq.Id = @requestId
				)
	END

	SELECT WorkDate
	FROM #tmpCalendar

	DROP TABLE #tmpCalendar
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetWeekWorking]
	@id int,
	@date datetime
AS
BEGIN
	SET NOCOUNT ON;

	select * from EmployeeCalendar
	where EmpId = @id and datediff(week,WorkDate,@date)=0
	order by WorkDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetWorkDate] -- '2022-01-04 11:20:25.000'
    @EmpId int,
	@Year int
AS
/*
Created: 20230403@Brian.nguyen
Task: Get work date calendar
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	SELECT [WorkDate]      
	  FROM [NexHRM].[dbo].[EmployeeCalendar]
	  WHERE EmpId = @EmpId and (IsHoliday = 1 OR IsOffDay = 1) and Year(WorkDate) = @Year
	  GROUP BY [WorkDate]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_GetWorkingSchedule]
    @FromDate DATETIME
    ,@ToDate DATETIME
    ,@EmployeeId INT = NULL
    ,@GroupId INT = NULL
    ,@DepartmentId INT = NULL
AS
/*
Created: 20160219@Quinn.Vo
Task: Get employee working schedule by date.
DB: NexEPA
Original: 

Revisions:  
    - 20250515@Henry.Dinh: Add department
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT cl.WorkDate
       ,(CASE WHEN cl.IsLeaveDay = 1 AND cl.LeaveDay = 0.5 THEN 1 ELSE 0 END) AS IsLeaveHalfDay
       ,(CASE 
            WHEN cl.isHoliday = 0
                AND IsOffDay = 0
                AND IsLeaveDay = 0 THEN s.code
            WHEN cl.isHoliday = 1 THEN 'PH'
            WHEN isOffday = 1 THEN 'OFF'
            WHEN isLeaveDay = 1 THEN lv.Code + CASE 
                    WHEN LeaveDay = 0.5 THEN '½'
                    WHEN LeaveTypeId2 IS NOT NULL THEN '/' + lv2.Code
                    WHEN LeaveDay = 1 THEN ''
                    ELSE ''
                    END
            END) AS Title
         ,cl.EmpId
        ,e.NickName
        ,e.FullName
        ,d.Name AS DepartmentName
        ,e.GroupId AS TeamId
        ,g.Code AS TeamCode
        ,g.Name AS TeamName
    FROM dbo.Employee AS e WITH (NOLOCK)
    INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON e.Id = cl.EmpId
    INNER JOIN dbo.EmployeeGroup AS g ON g.Id = e.GroupId
    INNER JOIN dbo.Department AS d ON d.DepartmentId = g.DepartmentId
    LEFT JOIN dbo.LeaveType AS lv ON lv.id = cl.LeaveTypeId
     LEFT JOIN dbo.LeaveType AS lv2 ON lv2.Id = cl.LeaveTypeId2
     LEFT JOIN dbo.[Shift] AS s ON s.Id = cl.ShiftId
    WHERE e.IsSpecialUser = 0
		AND e.Enabled = 1
		AND e.isDeleted = 0
		AND cl.WorkDate BETWEEN @fromDate
            AND @toDate
        AND (
            @EmployeeId IS NULL
            OR e.Id = @EmployeeId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
        AND (
            @DepartmentId IS NULL
            OR g.DepartmentId = @DepartmentId
            )
    ORDER BY d.Name
        ,g.[Rank]
        ,g.Code
        ,Roles DESC
        ,e.GroupDate
        ,cl.WorkDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_Insert] @empid INT = NULL
	,@fromdate DATETIME
	,@todate DATETIME
	,@setOffDay BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/06@Dan
Task: Insert default working calendar for employee
DB: HRMS
Original: 

Revisions:    
Param's Explanation:

*/
-- cut off time
SET @fromdate = convert(VARCHAR(10), @fromdate, 111)
SET @todate = convert(VARCHAR(10), @todate, 111)

DECLARE @dw INT
	,@isHoliday BIT

WHILE @fromdate <= @todate
BEGIN
	SET @isHoliday = 0
	SET @dw = datepart(dw, @fromdate)

	--print @dw
	-- kiem tra @fromdate co phai la ngay nghi
	IF (
			EXISTS (
				SELECT *
				FROM dbo.Holiday WITH (NOLOCK)
				WHERE @fromdate BETWEEN FromDate
						AND ToDate
				)
			)
		SET @isHoliday = 1;

	-- se xu ly leave request o day neu can
	
	-- check neu da co data thi Update
	IF (EXISTS (SELECT *
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE WorkDate = @fromdate))
		BEGIN
		UPDATE dbo.EmployeeCalendar WITH (UPDLOCK, ROWLOCK)
		SET IsHoliday = @isHoliday
									,isOffDay = CASE WHEN @isHoliday = 1 THEN 0
													WHEN ShiftId IS NULL THEN 1
													WHEN @setOffDay = 0 THEN 0	
													WHEN @setOffDay = 1 THEN 1
													ELSE 0 END
									,ModifiedDate = dbo.fnc_GetClientDateTime()
									WHERE (WorkDate = @fromdate) AND (@empid is null OR EmpId = @empid)
		END	
	-- insert vao working calendar
	INSERT INTO dbo.EmployeeCalendar (
		EmpID
		,WorkDate
		,ShiftID
		,isOffDay
		,IsHoliday
		,ModifiedDate
		)
	SELECT empid
		,@fromdate AS WorkDate
		,CASE 
			WHEN @dw = 1 THEN SunShift
			WHEN @dw = 2 THEN MonShift
			WHEN @dw = 3 THEN TueShift
			WHEN @dw = 4 THEN WedShift
			WHEN @dw = 5 THEN ThuShift
			WHEN @dw = 6 THEN FriShift
			WHEN @dw = 7 THEN SatShift
			END AS ShiftID
		,CASE 
			WHEN @isHoliday = 1 THEN 0
			WHEN @setOffDay = 1 THEN 1
			WHEN @dw = 1
				AND SunShift IS NULL THEN 1
			WHEN @dw = 2
				AND MonShift IS NULL THEN 1
			WHEN @dw = 3
				AND TueShift IS NULL THEN 1
			WHEN @dw = 4
				AND WedShift IS NULL THEN 1
			WHEN @dw = 5
				AND ThuShift IS NULL THEN 1
			WHEN @dw = 6
				AND FriShift IS NULL THEN 1
			WHEN @dw = 7
				AND SatShift IS NULL THEN 1
			ELSE 0
			END AS isOffDay
		,@isHoliday
		,dbo.fnc_GetClientDateTime()
	FROM dbo.EmployeeShift s WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK)
			ON e.id = s.empid AND e.isDeleted = 0 AND e.IsSpecialUser = 0
	WHERE
		-- isnull(IsDeleted,0)=0 and 
		(
			@empid IS NULL
			OR empid = @empid
			)
		-- AND isnull(ResignedDate, '2079/01/01') >= @fromdate
		AND NOT EXISTS (
			SELECT *
			FROM EmployeeCalendar c WITH (NOLOCK)
			WHERE WorkDate = @fromdate
				AND s.empid = c.empid
			)

	SET @fromdate = dateadd(d, 1, @fromdate)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_SetOffDayForAllEmployees]
	@date datetime,
	@isOffDay bit,
	@ModifiedBy int,
	@ModifiedDate datetime ,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT		
AS	

/*
Created: 2012/07/19@Dan
Task: Turn on/off the off day on working calendar for all employees. 
	It's used to set replacement off day for public holiday when public holiday is on the off day
DB: NexHRM
Original: 

Revisions:    
Param's Explanation:

*/

SET NOCOUNT ON;  
IF (@isOffDay IS NULL)
	SET @isOffDay=0
	
-- if not exist calendar

IF (NOT EXISTS(SELECT *
				FROM dbo.EmployeeCalendar WITH (NOLOCK)
				WHERE workdate = @date					
					)
	)
BEGIN
	SET @ErrCode = 100
	SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'
	RETURN
END

-- cut off time

 set @date = convert(varchar(10),@date,111)  
  
 UPDATE dbo.EmployeeCalendar  
 SET isOffDay = case when @isOffDay = 0 and ShiftID is null and isHoliday=0 and isLeaveday=0 then 1
					else @isOffDay end,
	ModifiedBy = @ModifiedBy,
	ModifiedDate = dbo.fnc_GetClientDateTime()					
 WHERE WorkDate = @date
    
 -- update attendate record   
 EXEC EmployeeAttendance_ProcessingAndInsert null, @date, @date, 1  
 
 -- write log
Declare @LogSPData varchar(4000), @now datetime
set @now=dbo.fnc_GetClientDateTime()
--Log general info
Select @LogSPData = '<EmployeeCalendar_SetOffDayForAllEmployees @date="'+ coalesce(convert(varchar(10),@date, 111 ),space(1)) + '" ' +
'isOffDay="'+ coalesce(cast(@isOffDay as varchar(100)),space(1)) + '" ' +
'@ModifiedBy="'+ coalesce(cast(@ModifiedBy as varchar(100)),space(1)) + '" ' +
'@ModifiedDate="'+ coalesce(convert(varchar(10),@ModifiedDate, 111 ),space(1)) + 
'@ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
'@ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'	

EXEC dbo.Log_WriteLog
	@LogActionID = 4,
	@LogDate = @now,
	@Description = 'Set off day for all employees',
	@TrackingValue = @LogSPData,
	@UserID = @ModifiedBy 

set @ErrCode=0 
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_Update] @EmployeeId INT
    ,@Date DATETIME
    ,@ShiftId INT = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*  
Created: 2012/05/10@Dan  
Task: Update working calendar for an employee in a day  
DB: NexHR  
Original:   
  
Revisions:      
Param's Explanation:  
 @EmpId: employee ID  
 @date: the date to update  
 @ShiftID: shift id (null if not work)  
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @fromdate DATETIME
        ,@todate DATETIME
        ,@isHoliday BIT

    SET @Date = CONVERT(VARCHAR(10), @Date, 111)

    UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
    SET @isHoliday = CASE 
            WHEN EXISTS (
                    SELECT 1
                    FROM dbo.Holiday WITH (NOLOCK)
                    WHERE WorkDate BETWEEN FromDate
                            AND ToDate
                    ) THEN 1
            ELSE 0
            END
        ,IsHoliday = CASE 
            WHEN @ShiftId IS NULL THEN @isHoliday
            ELSE 0
            END
        ,ShiftId = @ShiftId
        ,isOffDay = CASE 
            WHEN @ShiftID IS NULL
                AND @isHoliday = 0
                AND LeaveTypeId IS NULL THEN 1
            ELSE 0
            END
        ,ModifiedBy = @ModifiedBy
        ,ModifiedDate = dbo.fnc_GetClientDateTime()
    WHERE empId = @EmployeeId
        AND DATEDIFF(d, WorkDate, @date) = 0

    EXEC EmployeeAttendance_ProcessingAndInsert @EmployeeId
        ,@Date
        ,@Date
        ,1

      SELECT @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_UpdateWeekWorking] 
	@Id int, 
	@date datetime,
	@monShift int,
	@tueShift int,
	@wedShift int,
	@thuShift int,
	@friShift int,
	@satShift int,
	@sunShift int
AS
/*
Created: 2012/04/19@Dan
Task: Update working calendar for an employee in a week
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
	@Id: employee ID
	@date: the date to identify the week
	@monShift: shift id for Monday
	@tueShift: shift id for Tueday
	@wedShift: shift id for Wedday
	@thuShift: shift id for Thursday
	@friShift: shift id for Friday
	@satShift: shift id for Saturday
	@sunShift: shift id for Sunday

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromdate datetime,
			@todate datetime,
			@dw int,
			@isHoliday bit

	SET @fromdate= DATEADD(DD, 1 - DATEPART(DW, @date), @Date)

	SET @todate= DATEADD(d, 6, @fromdate)

	UPDATE dbo.EmployeeCalendar WITH (UPDLOCK, ROWLOCK)
	SET @dw=datepart(dw, WorkDate),
		@isHoliday =  case when EXISTS(select *	from dbo.Holiday WITH (NOLOCK)
									where WorkDate between FromDate and ToDate)
							THEN 1 ELSE 0 END,
		IsHoliday = Case when @dw=1 AND @SunShift IS NULL THEN @isHoliday
						when @dw=2 AND @MonShift IS NULL THEN @isHoliday
						when @dw=3 AND @TueShift IS NULL THEN @isHoliday
						when @dw=4 AND @WedShift IS NULL THEN @isHoliday
						when @dw=5 AND @ThuShift IS NULL THEN @isHoliday
						when @dw=6 AND @FriShift IS NULL THEN @isHoliday
						when @dw=7 AND @SatShift IS NULL THEN @isHoliday
						ELSE 0
					end ,
		ShiftId =  Case when @dw=1 then @SunShift
						when @dw=2 then @MonShift
						when @dw=3 then @TueShift
						when @dw=4 then @WedShift
						when @dw=5 then @ThuShift
						when @dw=6 then @FriShift
						when @dw=7 then @SatShift
					end ,
		isOffDay = case when @dw=1 and @SunShift is null and @isHoliday=0 and LeaveTypeId is null then 1
						when @dw=2 and @MonShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						when @dw=3 and @TueShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						when @dw=4 and @WedShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						when @dw=5 and @ThuShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						when @dw=6 and @FriShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						when @dw=7 and @SatShift is null and @isHoliday=0 and LeaveTypeId is null  then 1
						else 0
					end,
		ModifiedDate = dbo.fnc_GetClientDateTime()
	WHERE empId = @Id AND
		WorkDate BETWEEN @fromdate AND @todate

	-- update attendance record 
	EXEC EmployeeAttendance_ProcessingAndInsert @id, @fromdate, @todate, 1

END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@IsWorkFromHome BIT
AS
/*
Created: 20211227@Teddy.le
Task: EmployeeCalendar Update Work From Home
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	IF EXISTS (
			SELECT 1
			FROM [dbo].[EmployeeCalendar] WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND ( WorkDate BETWEEN @FromDate AND @ToDate )
			)
	BEGIN
		UPDATE [dbo].[EmployeeCalendar] WITH (ROWLOCK, UPDLOCK)
		SET IsWorkFromHome = @IsWorkFromHome, ModifiedDate = dbo.fnc_GetClientDateTime()
		WHERE EmpId = @EmpId  
		AND ( WorkDate BETWEEN @FromDate AND @ToDate )
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeConversation_Get]
AS
/*
Created: 20230404@Brian.nguyen
Task: Get all conversation of employee
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT e.Id 
        ,e.NickName 
		,ec.ConversationId
		--,'29:19TyupFcn7CJOya4b7d76pGpzpmppzjOzppdk8JttHuU' AS ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK)
			ON e.Id = ec.EmpId
    WHERE  e.IsSpecialUser = 0 
		AND e.isDeleted = 0
		AND e.Enabled = 1
		-- AND e.GroupId = 9
		-- AND e.Id = 125
		-- AND e.NickName = 'Frankie'
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeConversation_GetById]
	@EmpId INT
AS
/*
Created: 20230404@Brian.nguyen
Task: Get conversation of employee from employee id
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT e.Id AS EmpId
        ,e.NickName 
		,ec.ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK)
			ON e.Id = ec.EmpId
    WHERE  e.isDeleted = 0
		AND e.Enabled = 1
		AND e.Id = @EmpId

END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeConversation_GetTeamHr]
AS
/*
	Created: 2023@Brian.nguyen
	Task: Get group conversations team human resources
	DB: NexHRM
	Revisions: 
		20250507@Henry.Dinh: update group id from 38 to 1104
	Original: 

*/
BEGIN
	SET NOCOUNT ON; 

	SELECT e.Id 
        ,e.NickName 
		,ec.ConversationId
		--,'29:1KmVXImsgANnLEyWFPGem8Y4K4fMFvYN-tE_QfuS0SfASNxMSmouUgyega3ebG6yZ' AS ConversationId
    FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeConversation AS ec WITH (NOLOCK)
			ON e.Id = ec.EmpId
    WHERE  e.IsSpecialUser = 0 
		AND e.isDeleted = 0
		AND e.Enabled = 1
		AND GroupId = 1104
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeConversation_Mapping] 
AS
/*
Created: 20140604@Quinn.Vo
Task: Insert Leave deduction.
DB: NexHRM
Original: 

Revisions: 
	- 20250317@Henry.Dinh: Get latest conversationid by ModifiedDate
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @PrefixDomain VARCHAR(50) = 'spvn\'
		,@EmailDomain VARCHAR(50) = '@starixsoft.com'
		,@BotName VARCHAR(50) = 'nexhrbot'

	DECLARE @ModifiedDate DATETIME
	CREATE TABLE #tblConversationUser(
		ConversationId VARCHAR(255)
		,Email VARCHAR(500)
	)

	SELECT @ModifiedDate = MAX(ISNULL(ModifiedDate, '2020-01-01'))
	FROM dbo.EmployeeConversation WITH (NOLOCK)

	INSERT INTO #tblConversationUser(ConversationId, Email)
	SELECT 
        ConversationId,
        UserName
    FROM (
        SELECT 
            co.ConversationId,
            cou.UserName,
            ROW_NUMBER() OVER (
				PARTITION BY REPLACE(cou.UserName, @EmailDomain, '')
				ORDER BY co.ModifiedDate DESC
   			) as RN
        FROM NexBot.dbo.Conversation AS co WITH (NOLOCK)
        INNER JOIN NexBot.dbo.ConversationUser AS cou WITH (NOLOCK) 
            ON co.ConversationId = cou.ConversationId
        INNER JOIN NexBot.dbo.ConversationBot AS bot WITH (NOLOCK) 
            ON co.ConversationBotId = bot.ConversationBotId
        WHERE bot.Name = @BotName
    ) t
    WHERE t.RN = 1

	UPDATE ec
	SET ec.ConversationId = cou.ConversationId
		,ec.ModifiedDate = dbo.fnc_GetClientDateTime()
	FROM dbo.EmployeeConversation AS ec
	INNER JOIN dbo.Employee AS e ON ec.EmpId = e.Id
	INNER JOIN #tblConversationUser AS cou ON 
	REPLACE(ISNULL(e.DomainUsername, ''), @PrefixDomain, '') = REPLACE(cou.Email, @EmailDomain, '')
	WHERE ec.ConversationId <> cou.ConversationId

	INSERT INTO dbo.EmployeeConversation(EmpId, ConversationId, ModifiedDate)
	SELECT e.Id, cou.ConversationId, dbo.fnc_GetClientDateTime()
	FROM #tblConversationUser AS cou
	INNER JOIN dbo.Employee AS e WITH (NOLOCK)
		ON e.isDeleted = 0
		AND REPLACE(ISNULL(e.DomainUsername, ''), @PrefixDomain, '') = REPLACE(cou.Email, @EmailDomain, '')
	LEFT JOIN dbo.EmployeeConversation AS ec ON ec.EmpId = e.Id
	WHERE ec.EmpId IS NULL

	DROP TABLE #tblConversationUser
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeExt_ChangePassword] @Id INT
    ,@Password NVARCHAR(100)
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
    Created: 20250507@Henry.Dinh
    Task: change password
    DB: NexHRM
    Original: 

    Revisions:    
        20250507@Henry.Dinh: change password
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.EmployeeExt WHERE Id = @Id)
    BEGIN
        SET @ErrCode = 1
        SET @ErrMsg = 'Employee not found'
        RETURN
    END

    UPDATE dbo.EmployeeExt
    SET Password = @Password
    WHERE Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeExt_GetLoginEmployee] @DomainUsername VARCHAR(50)
AS
/*
    Created: 20250507@Henry.Dinh
    Task: log in
    DB: HRM
    Original: 
    Revisions:    
    Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SELECT Id, DomainUsername, Password, IsDeleted, Enabled
    FROM EmployeeExt WHERE DomainUsername = @DomainUsername
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_Delete] @Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/06/11@Dan
Task: soft delete a group 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeeGroup_Employee eg WITH (NOLOCK)
                    INNER JOIN Employee e WITH (NOLOCK) ON eg.EmpID = e.ID
                WHERE eg.GroupID = @Id
                    AND ISNULL(isDeleted, 0) = 0
                )
            )
    BEGIN
        SELECT @ErrCode = 100
            ,@ErrMsg = 'The group is not empty so cannot delete it'

        RETURN
    END

    UPDATE [dbo].[EmployeeGroup] WITH (ROWLOCK, UPDLOCK)
    SET Deleted = 1
        ,ModifiedDate = dbo.fnc_GetClientDateTime()
        ,ModifiedBy = @ModifiedBy
    WHERE Id = @Id

    SET @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_DeleteMember]
	@EmpId INT
   ,@GroupId INT   
AS
/*
Created: 2012/06/11@Dan
Task: delete member from group (not allow delete maingroup)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN	
	DELETE dbo.EmployeeGroup_Employee	
	WHERE EmpID=@EmpID AND GroupId=@GroupId AND isMainGroup=0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_Get] 
    @FromDate DATETIME = NULL
AS
/*
Created: 20140320@Quinn.Vo
Task: Get all groups from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT 
    g.Id AS TeamId,
    g.DepartmentId,
    g.Code,
    g.Name,
    ISNULL(g.Deleted, 0) AS IsDeleted,
    CASE WHEN g.ModifiedDate IS NULL THEN g.CreatedDate ELSE g.ModifiedDate END AS ModifiedDate
    FROM dbo.EmployeeGroup AS g WITH (NOLOCK)
    WHERE (@FromDate IS NULL
    OR g.ModifiedDate > @FromDate)
	AND g.IsSpecialTeam = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_GetById] @id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT g.[Id]
        ,g.[Code]
        ,g.[Name]
        ,g.[Rank]
        ,g.[Deleted]
        ,g.ParentId
        ,g.[DepartmentId]
    FROM dbo.EmployeeGroup g WITH (NOLOCK)
    WHERE g.Id = @id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_GetGroupOfEmployee]
	@employeeId int
AS
BEGIN
	SET NOCOUNT ON;

	select * from EmployeeGroup
	where Id= (	select top(1) GroupId from Employee 
				where Id = @employeeId)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_GetGroups] 
	@IncludeSpecialTeam BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
	SET @IncludeSpecialTeam = ISNULL(@IncludeSpecialTeam, 0)

    SELECT g.Id,
		g.Id as TeamId,
		g.Name, 
        d.DepartmentId,
        d.Name as DepartmentName,
		g.Code, 
		g.Rank, 
        COUNT(e.Id) AS NoOfMembers
    FROM dbo.EmployeeGroup g WITH (NOLOCK)
        LEFT JOIN dbo.Employee e WITH (NOLOCK) ON e.GroupId = g.Id AND e.isDeleted = 0 AND e.IsSpecialUser = 0
        LEFT JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
    WHERE ((@IncludeSpecialTeam = 0 AND g.IsSpecialTeam = 0) OR @IncludeSpecialTeam = 1)  
		AND ISNULL(Deleted,0)=0
    GROUP BY g.Id, g.Name, g.Code, g.Rank, d.DepartmentId, d.Name
    ORDER BY g.[Code]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_GetGroupsByManagerID] 
    @managerID int
AS
/*
Created: 2012/05/18@Dan
Task: Get group of leader or assistant leader
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT g.[Id]
		,g.[Id]
        ,g.[Code]
        ,g.[Name]
        ,g.[Rank]
        ,g.[Deleted]
        ,g.ParentId
        ,g.[DepartmentId]
    FROM dbo.EmployeeGroup AS g WITH (NOLOCK)
	INNER JOIn dbo.GroupSupervisor AS gs WITH (NOLOCK) ON g.Id = gs.GroupId AND gs.IsDeleted = 0
    WHERE 
		gs.ApproverId = @managerID
		AND ISNULL(Deleted,0)=0
    ORDER BY [Name]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_Insert] @Code VARCHAR(20)
    ,@Name VARCHAR(50)
    ,@DepartmentId INT = NULL
    ,@ParentId INT = NULL
    ,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/06/11@Dan
Task: insert group
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeeGroup WITH (NOLOCK)
                WHERE (Code = @Code
                    OR NAME = @Name)
                    AND ISNULL(Deleted, 0) = 0
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This team has already existed.'

        RETURN
    END

    INSERT INTO dbo.EmployeeGroup (
        Code
        ,NAME
        ,DepartmentId
        ,CreatedDate
        ,CreatedBy
        ,ModifiedDate
        ,ParentId
        )
    VALUES (
        @Code
        ,@Name
        ,@DepartmentId
        ,dbo.fnc_GetClientDateTime()
        ,@CreatedBy
        ,dbo.fnc_GetClientDateTime()
        ,@ParentId
        )
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_InsertMember]
	@EmpId INT
   ,@GroupId INT
   ,@isMainGroup bit
AS
/*
Created: 2012/06/11@Dan
Task: insert member into group
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	INSERT INTO dbo.EmployeeGroup_Employee(GroupId, EmpID, isMainGroup)
	VALUES (@GroupId, @EmpId, @isMainGroup)

	IF (@isMainGroup=1)
	BEGIN
		UPDATE dbo.EmployeeGroup_Employee
		SET isMainGroup=0
		WHERE EmpID=@EmpID AND GroupId<>@GroupId
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeGroup_Update] @Id INT
    ,@Code VARCHAR(20)
    ,@Name NVARCHAR(50)
    ,@DepartmentId INT = NULL
    ,@ParentId INT = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/06/11@Dan
Task: update group 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeeGroup WITH (NOLOCK)
                WHERE Id <> @Id
                    AND (
                        Code = @Code
                        OR NAME = @Name
                        )
                    AND ISNULL(Deleted, 0) = 0
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This team has already existed.'

        RETURN
    END

    UPDATE [dbo].[EmployeeGroup] WITH (ROWLOCK, UPDLOCK)
    SET Code = @Code
        ,[Name] = @Name
        ,DepartmentId = @DepartmentId
        ,ParentId = @ParentId
        ,ModifiedDate = dbo.fnc_GetClientDateTime()
        ,ModifiedBy = @ModifiedBy
    WHERE Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeJobGrade_Get] 
	 @GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@IsOnlyCurrentUsed BIT = 1
AS
/*
Created: 20240104@Quinn.Vo
Task: Get Employee Job Grade
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.Id
		,e.Code
		,e.NickName
		,e.FullName
		,e.GroupId
		,eg.Code AS TeamCode
		,eg.Name AS TeamName
		,gra.JobGradeId
		,gra.Code AS JobGradeCode
		,gra.Name AS JobGradeName
		,gra.AffectedDate
		,gra.Description
		,gra.Note
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.JobGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeJobGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.JobGrade as jg WITH (NOLOCK) on ejg.JobGradeId = jg.JobGradeId
			  WHERE ((@IsOnlyCurrentUsed = 1 AND ejg.IsCurrentUsed = 1) OR (@IsOnlyCurrentUsed = 0))
				AND ISNULL(ejg.IsDeleted,0) = 0 
		) AS gra ON e.Id = gra.EmpId
	WHERE e.isDeleted = 0
	AND e.IsSpecialUser = 0
	AND  (@GroupId IS NULL
				OR e.GroupId = @GroupId
            )
			AND (
				@EmployeeId IS NULL
				OR e.Id = @EmployeeId
            )
	ORDER BY eg.[Rank]
	,eg.Code
	,e.Roles DESC
	,e.GroupDate
	,gra.AffectedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeLogTimeMissed_UpdateMultiple]
    @EmpIDs NVARCHAR(MAX) 
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20250228@Henry.dinh
Task: Insert/Update missed logging by date
DB: NexHRM
Original: EmployeeLogTimeMissed_UpsertMultipleByDate
Revisions: 
    - 20250307 - Changed to use Date instead of Month/Year
Param's Explanation:
- @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
- @ErrMsg: Error description.
*/
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @Yesterday DATE = 
            CASE 
                WHEN DATEPART(WEEKDAY, dbo.fnc_GetClientDateTime()) = 2 THEN
                    DATEADD(DAY, -3, dbo.fnc_GetClientDateTime())
                ELSE
                    DATEADD(DAY, -1, dbo.fnc_GetClientDateTime())
            END;
            
        DECLARE @EmpTable TABLE (EmpID INT);
        
        INSERT INTO @EmpTable (EmpID) SELECT value FROM STRING_SPLIT(@EmpIDs, ',');
        
        INSERT INTO [dbo].[EmployeeLogTimeMissed] (EmpID, [Date])
        SELECT source.EmpID, @Yesterday
        FROM @EmpTable AS source
        LEFT JOIN [dbo].[EmployeeLogTimeMissed] AS target
            ON target.EmpID = source.EmpID AND target.[Date] = @Yesterday
        WHERE target.EmpID IS NULL;
            
        SET @ErrCode = 0;
        SET @ErrMsg = '';
    END TRY
    BEGIN CATCH
        SET @ErrCode = ERROR_NUMBER();
        SET @ErrMsg = ERROR_MESSAGE();
    END CATCH
    
    SET NOCOUNT OFF;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeManagementGrade_Get] 
	 @GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@IsOnlyCurrentUsed BIT = 1
AS
/*
Created: 20252602@Henry.Dinh
Task: Get Employee Management Grade
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.Id
		,e.Code
		,e.NickName
		,e.FullName
		,e.GroupId
		,eg.Code AS TeamCode
		,eg.Name AS TeamName
		,gra.ManagementGradeId
		,gra.Code AS ManagementGradeCode
		,gra.Name AS ManagementGradeName
		,gra.AffectedDate
		,gra.Description
		,gra.Note
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	LEFT JOIN (
			SELECT ejg.EmpId
			,ejg.AffectedDate
			,ejg.Note
			,jg.ManagementGradeId
			,jg.Code
			,jg.Name
			,jg.Description
			  FROM HRM.dbo.EmployeeManagementGrade as ejg  WITH (NOLOCK)
			  INNER JOIN HRM.dbo.ManagementGrade as jg WITH (NOLOCK) on ejg.ManagementGradeId = jg.ManagementGradeId
			  WHERE ((@IsOnlyCurrentUsed = 1 AND ejg.IsCurrentUsed = 1) OR (@IsOnlyCurrentUsed = 0))
				AND ISNULL(ejg.IsDeleted,0) = 0 
		) AS gra ON e.Id = gra.EmpId
	WHERE e.isDeleted = 0
	AND e.IsSpecialUser = 0
	AND  (@GroupId IS NULL
				OR e.GroupId = @GroupId
            )
			AND (
				@EmployeeId IS NULL
				OR e.Id = @EmployeeId
            )
	ORDER BY eg.[Rank]
	,eg.Code
	,e.Roles DESC
	,e.GroupDate
	,gra.AffectedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeePostnatal_Delete] @EmpId INT
    ,@FromDate DATETIME
    ,@ToDate DATETIME
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeePostnatal ep
                WHERE ep.EmpId = @EmpId
                    AND ep.FromDate = @FromDate
                    AND ep.ToDate = @ToDate
                )
            )
    BEGIN
        DELETE dbo.EmployeePostnatal
        WHERE dbo.EmployeePostnatal.EmpId = @EmpId
            AND dbo.EmployeePostnatal.FromDate = @FromDate
            AND dbo.EmployeePostnatal.ToDate = @ToDate

        IF @@ERROR <> 0
        BEGIN
            SELECT @ErrCode = 1
                ,@ErrMsg = N'Delete Employee Postnatal failed.';
        END;
        ELSE
        BEGIN
            SELECT @ErrCode = 0
                ,@ErrMsg = '';
        END;
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'Cannot find this Employee Postnatal !'
    END
END;

GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeePostnatal_GetAll] @EmpId INT
    ,@Year INT
AS
BEGIN
    SELECT ep.EmpId
        ,e.NickName
        ,e.FullName
        ,ep.FromDate
        ,ep.ToDate
        ,e2.NickName AS 'ModifiedBy'
        ,ep.ModifiedDate
        ,ep.Minutes
        ,ep.Type
    FROM dbo.EmployeePostnatal ep
        JOIN dbo.Employee e ON ep.EmpId = e.Id
        LEFT JOIN dbo.Employee e2 ON ep.ModifiedBy = e2.Id
    WHERE (
            @EmpId = -1
            OR ep.EmpId = @EmpId
            )
        AND (
            @Year = - 1
            OR (
                YEAR(ep.FromDate) = @Year
                OR YEAR(ep.ToDate) = @Year
                )
            )
	ORDER BY ep.ToDate DESC
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeePostnatal_GetById] @EmpId INT
    ,@FromDate DATETIME
    ,@ToDate DATETIME
AS
BEGIN
    SELECT ep.EmpId
        ,e.NickName
        ,ep.FromDate
        ,ep.ToDate
        ,ep.ModifiedBy
        ,ep.ModifiedDate
        ,ep.Minutes
        ,ep.Type
    FROM dbo.EmployeePostnatal ep
        JOIN dbo.Employee e ON ep.EmpId = e.Id
    WHERE ep.EmpId = @EmpId
        AND ep.FromDate = @FromDate
        AND ep.ToDate = @ToDate
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeePostnatal_Insert] @EmpId INT
    ,@FromDate DATETIME
    ,@ToDate DATETIME
    ,@ModifiedBy INT
    ,@Minutes INT
    ,@Type tinyint
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeePostnatal ep
                WHERE ep.EmpId = @EmpId
                    AND ep.FromDate = @FromDate
                    AND ep.ToDate = @ToDate
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This request has already existed !'

        RETURN
    END

    INSERT INTO dbo.EmployeePostnatal (
        EmpId
        ,FromDate
        ,ToDate
        ,ModifiedBy
        ,ModifiedDate
        ,[Minutes]
        ,[Type]
        )
    VALUES (
        @EmpId
        ,@FromDate
        ,@ToDate
        ,@ModifiedBy
        ,dbo.fnc_GetClientDateTime()
        ,@Minutes
        ,@Type
        );

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Create new Employee Postnatal failed.';
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = '';
    END;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeePostnatal_Update] @OldEmpId INT
    ,@OldFromDate DATETIME
    ,@OldToDate DATETIME
    ,@NewEmpId INT
    ,@NewFromDate DATETIME
    ,@NewToDate DATETIME
    ,@ModifiedBy INT
    ,@Minutes INT
    ,@Type tinyint
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeePostnatal ep
                WHERE ep.EmpId = @OldEmpId
                    AND ep.FromDate = @OldFromDate
                    AND ep.ToDate = @OldToDate
                )
            )
    BEGIN
        UPDATE dbo.EmployeePostnatal
        SET dbo.EmployeePostnatal.EmpId = @NewEmpId
            ,dbo.EmployeePostnatal.FromDate = @NewFromDate
            ,dbo.EmployeePostnatal.ToDate = @NewToDate
            ,dbo.EmployeePostnatal.ModifiedBy = @ModifiedBy
            ,dbo.EmployeePostnatal.ModifiedDate = dbo.fnc_GetClientDateTime()
            ,dbo.EmployeePostnatal.Minutes = @Minutes
            ,dbo.EmployeePostnatal.Type = @Type
        WHERE dbo.EmployeePostnatal.EmpId = @OldEmpId
            AND dbo.EmployeePostnatal.FromDate = @OldFromDate
            AND dbo.EmployeePostnatal.ToDate = @OldToDate

        IF @@ERROR <> 0
        BEGIN
            SELECT @ErrCode = 1
                ,@ErrMsg = N'Create new Employee Postnatal failed.';
        END;
        ELSE
        BEGIN
            SELECT @ErrCode = 0
                ,@ErrMsg = '';
        END;
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'Cannot find this Employee Postnatal !'
    END
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeSetting_Get]
				@employeeId INT
AS
/*
	Created: 20180618@Oliver.Dang
	Task: Get employee setting by employee id
	DB: NexHRM
	Original: 

	Revisions:  
		20250507@Henry.Dinh: update group id from 38 to 1104
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
		
			IF EXISTS (SELECT 1 FROM [dbo].[Employee] WITH (NOLOCK) WHERE Id = @employeeId
				AND (GroupId IN (40, 1, 1104, 2, 11) OR CompanyId = 2)
			)
			BEGIN
				SELECT @employeeId AS [EmpId]
				,1 AS [DisplayInfoAndTodoList]
				,1 AS [DisplayTeamAttendence]
				,0 AS [DisplayTeamWorkLoad]
				,0 AS [DisplayRedmineTask]
				,0 AS [DisplayRedmineInputSpendTime]
				,0 AS [DisplayRemindLogtime]
			END
			ELSE IF EXISTS (SELECT 1 FROM [dbo].[Employee] WITH (NOLOCK) WHERE Id = @employeeId
				AND (GroupId IN (51, 63, 13, 44, 54)
				 OR Roles IN ('1', '2', '4'))
			)
			BEGIN
				
				IF NOT EXISTS (SELECT 1 FROM [dbo].[EmployeeSetting]  WITH (NOLOCK) WHERE EmpId = @employeeId)
				BEGIN
					SELECT @employeeId AS [EmpId]
					,1 AS [DisplayInfoAndTodoList]
					,1 AS [DisplayTeamAttendence]
					,1 AS [DisplayTeamWorkLoad]
					,1 AS [DisplayRedmineTask]
					,1 AS [DisplayRedmineInputSpendTime]
					,0 AS [DisplayRemindLogtime]
				END
				ELSE
				BEGIN
					SELECT [EmpId]
				  ,[DisplayInfoAndTodoList]
				  ,[DisplayTeamAttendence]
				  ,[DisplayTeamWorkLoad]
				  ,[DisplayRedmineTask]
				  ,[DisplayRedmineInputSpendTime]
				  ,[DisplayRemindLogtime] 
			  FROM dbo.EmployeeSetting WITH (NOLOCK) WHERE EmpId = @employeeId
				END

				
			END
			ELSE
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM [dbo].[EmployeeSetting]  WITH (NOLOCK) WHERE EmpId = @employeeId)
				BEGIN
					SELECT @employeeId AS [EmpId]
						,1 AS [DisplayInfoAndTodoList]
						,1 AS [DisplayTeamAttendence]
						,1 AS [DisplayTeamWorkLoad]
						,1 AS [DisplayRedmineTask]
						,1 AS [DisplayRedmineInputSpendTime]
						,1 AS [DisplayRemindLogtime]
				END
				ELSE
				BEGIN
					SELECT [EmpId]
				  ,[DisplayInfoAndTodoList]
				  ,[DisplayTeamAttendence]
				  ,[DisplayTeamWorkLoad]
				  ,[DisplayRedmineTask]
				  ,[DisplayRedmineInputSpendTime]
				  ,[DisplayRemindLogtime] 
			  FROM dbo.EmployeeSetting WITH (NOLOCK) WHERE EmpId = @employeeId
				END
			END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeShift_Get]
    @EmployeeId INT
AS
/*
Created: 20160116@Quinn.Vo
Task: Get employee default schedule.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/

BEGIN
    SET NOCOUNT ON;

    SELECT es.EmpID,
       e.NickName,
       e.FullName,
       [MonShift] AS MonShiftId,
        ISNULL(s1.Name, 'OFF') AS MonShiftName,
        [TueShift] AS TueShiftId,
        ISNULL(s2.Name, 'OFF') AS TueShiftName,
        [WedShift] AS WedShiftId,
        ISNULL(s3.Name, 'OFF') AS WedShiftName,
        [ThuShift] AS ThuShiftId,
        ISNULL(s4.Name, 'OFF') AS ThuShiftName,
        [FriShift] AS FriShiftId,
        ISNULL(s5.Name, 'OFF') AS FriShiftName,
        [SatShift] AS SatShiftId,
        ISNULL(s6.Name, 'OFF') AS SatShiftName,
        [SunShift] AS SunShiftId,
        ISNULL(s7.Name, 'OFF') AS SunShiftName
    FROM [dbo].[EmployeeShift] AS es WITH (NOLOCK)
       INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpID = e.Id
        LEFT JOIN dbo.[Shift] AS s1 WITH (NOLOCK) ON es.MonShift = s1.Id
        LEFT JOIN dbo.[Shift] AS s2 WITH (NOLOCK) ON es.TueShift = s2.Id
        LEFT JOIN dbo.[Shift] AS s3 WITH (NOLOCK) ON es.WedShift = s3.Id
        LEFT JOIN dbo.[Shift] AS s4 WITH (NOLOCK) ON es.ThuShift = s4.Id
        LEFT JOIN dbo.[Shift] AS s5 WITH (NOLOCK) ON es.FriShift = s5.Id
        LEFT JOIN dbo.[Shift] AS s6 WITH (NOLOCK) ON es.SatShift = s6.Id
        LEFT JOIN dbo.[Shift] AS s7 WITH (NOLOCK) ON es.SunShift = s7.Id
    WHERE EmpID = @EmployeeId;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeShift_Update]
	@employeeId INT
	,@monShift INT = NULL
	,@tueShift INT = NULL
	,@wedShift INT = NULL
	,@thuShift INT = NULL
	,@friShift INT = NULL
	,@satShift INT = NULL
	,@sunShift INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromdate DATETIME
		,@dw INT
		,@isHoliday BIT

	SET @fromdate = dbo.fnc_GetClientDateTime()

	-- Insert statements for procedure here
	UPDATE dbo.EmployeeShift WITH (UPDLOCK, ROWLOCK)
	SET MonShift = @monShift
		,TueShift = @tueShift
		,WedShift = @wedShift
		,ThuShift = @thuShift
		,FriShift = @friShift
		,SatShift = @satShift
		,SunShift = @sunShift
	WHERE EmpID = @employeeId

	UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
	SET @dw = datepart(dw, WorkDate)
		,@isHoliday = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.Holiday WITH (NOLOCK)
					WHERE WorkDate BETWEEN FromDate
							AND ToDate
					) THEN 1
			ELSE 0
			END
		,IsHoliday = @isHoliday
		,ShiftId = CASE 
			WHEN @dw = 1
				AND @isHoliday = 0 THEN @SunShift
			WHEN @dw = 2
				AND @isHoliday = 0 THEN @MonShift
			WHEN @dw = 3
				AND @isHoliday = 0 THEN @TueShift
			WHEN @dw = 4
				AND @isHoliday = 0 THEN @WedShift
			WHEN @dw = 5
				AND @isHoliday = 0 THEN @ThuShift
			WHEN @dw = 6
				AND @isHoliday = 0 THEN @FriShift
			WHEN @dw = 7
				AND @isHoliday = 0 THEN @SatShift
			ELSE ShiftId
			END
		,isOffDay = CASE 
			WHEN @dw = 1
				AND @SunShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 2
				AND @MonShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 3
				AND @TueShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 4
				AND @WedShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 5
				AND @ThuShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 6
				AND @FriShift IS NULL
				AND @isHoliday = 0 THEN 1
			WHEN @dw = 7
				AND @SatShift IS NULL
				AND @isHoliday = 0 THEN 1
			ELSE 0
			END
		,ModifiedDate = dbo.fnc_GetClientDateTime()
	WHERE empId = @employeeId
		AND DATEDIFF(dd, @fromdate, WorkDate) >= 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeSupervisor_Get]
AS
/*
Created: 20240426@Lilac.Nguyen
Task: Get Employee Supervisor.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
   
		SELECT
            es.EmpId,
			em.NickName,
			em.FullName,
			STRING_AGG(e.NickName, ', ') AS Approvers
        FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			LEFT JOIN dbo.Employee AS em WITH (NOLOCK) ON em.Id = es.EmpId
			LEFT JOIN dbo.Employee AS e ON e.Id = es.ApproverId
        WHERE ISNULL(es.IsDeleted, 0) = 0
		GROUP BY es.EmpId, em.NickName, em.FullName
    
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeSupervisor_GetById]
	@EmpId INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20240422@Lilac.Nguyen
Task: get approver by id.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
	  
	SELECT em.Id AS EmpId,
			em.NickName,
			em.FullName,
			STRING_AGG(CONVERT(NVARCHAR(MAX), es.ApproverId), ', ') AS ApproverIds
    FROM dbo.Employee AS em WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON em.Id = es.EmpId
    WHERE ISNULL(es.IsDeleted, 0) = 0 AND em.Id = @EmpId
	GROUP BY em.Id, em.NickName, em.FullName

END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeSupervisor_Insert]
	@EmpId INT
    ,@ApproverIds VARCHAR(255) = NULL
	,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20240426@Lilac.Nguyen
Task: Insert approver for employee.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	DECLARE @ModifiedDate DATETIME = dbo.fnc_GetClientDateTime(), @logData NVARCHAR(MAX);
	
	SELECT @logData = '<Setting_Old Approver="'+ COALESCE(STRING_AGG(ApproverId, ', '), SPACE(1)) + '" />' +  
	'<Setting_New Approver="'+ COALESCE(CAST(@ApproverIds AS VARCHAR(255)), SPACE(1)) + '" />'
	FROM dbo.EmployeeSupervisor WITH (NOLOCK)
	WHERE EmpId = @EmpId AND IsDeleted <> 1

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.EmployeeSupervisor WITH (NOLOCK)
                WHERE EmpId = @EmpId AND ISNULL(IsDeleted, 0) = 0
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Employee has already exists'

        RETURN
    END

	CREATE TABLE #tblSupervisor (ApproverId INT)

	INSERT INTO #tblSupervisor(
            ApproverId
            )
    SELECT val
    FROM dbo.fnc_strSplit(@ApproverIds, ',')

	UPDATE su
	SET su.IsDeleted = 1
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM dbo.EmployeeSupervisor AS su WITH (ROWLOCK, UPDLOCK)
	WHERE su.EmpId = @EmpId
		AND ISNULL(su.IsDeleted, 0) = 0
		AND NOT EXISTS (
			SELECT 1
			FROM #tblSupervisor AS sp
			WHERE sp.ApproverId = su.ApproverId
			)

	UPDATE su
	SET su.IsDeleted = 0
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM dbo.EmployeeSupervisor AS su WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblSupervisor AS sp ON sp.ApproverId = su.ApproverId
	WHERE su.EmpId = @EmpId
		AND su.IsDeleted = 1

    MERGE dbo.EmployeeSupervisor AS [target]
	USING #tblSupervisor AS [source] ON (
				[target].EmpId = @EmpId
				AND [target].ApproverId = [source].ApproverId
				)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			EmpId
			,ApproverId
			,ModifiedBy
			,ModifiedDate
			,IsDeleted
			)
		VALUES (
			@EmpId
			,[source].ApproverId
			,@ModifiedBy
			,@ModifiedDate
			,0
			);

	INSERT INTO dbo.[Log](LogActionID, LogDate, [Description], TrackingValue, UserID)
	VALUES(8, dbo.fnc_GetClientDateTime(), 'Insert Personal Flow Setting', @logData, @EmpId)

	DROP TABLE #tblSupervisor;

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeSupervisor_Update]
	@EmpId INT
    ,@ApproverIds VARCHAR(255) = NULL
	,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20240422@Lilac.Nguyen
Task: update approver.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = dbo.fnc_GetClientDateTime(), @logData NVARCHAR(MAX);
	
	SELECT @logData = '<Setting_Old Approver="'+ COALESCE(STRING_AGG(ApproverId, ', '), SPACE(1)) + '" />' +  
	'<Setting_New Approver="'+ COALESCE(CAST(@ApproverIds AS VARCHAR(255)), SPACE(1)) + '" />'
	FROM dbo.EmployeeSupervisor WITH (NOLOCK)
	WHERE EmpId = @EmpId AND IsDeleted <> 1

	CREATE TABLE #tblSupervisor (ApproverId INT)

	INSERT INTO #tblSupervisor(
            ApproverId
            )
    SELECT val
    FROM dbo.fnc_strSplit(@ApproverIds, ',')

	UPDATE su
	SET su.IsDeleted = 1
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM dbo.EmployeeSupervisor AS su WITH (ROWLOCK, UPDLOCK)
	WHERE su.EmpId = @EmpId
		AND ISNULL(su.IsDeleted, 0) = 0
		AND NOT EXISTS (
			SELECT 1
			FROM #tblSupervisor AS sp
			WHERE sp.ApproverId = su.ApproverId
			)

	UPDATE su
	SET su.IsDeleted = 0
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM dbo.EmployeeSupervisor AS su WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblSupervisor AS sp ON sp.ApproverId = su.ApproverId
	WHERE su.EmpId = @EmpId
		AND su.IsDeleted = 1

	MERGE dbo.EmployeeSupervisor AS [target]
	USING #tblSupervisor AS [source] ON (
				[target].EmpId = @EmpId
				AND [target].ApproverId = [source].ApproverId
				)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			EmpId
			,ApproverId
			,ModifiedBy
			,ModifiedDate
			,IsDeleted
			)
		VALUES (
			@EmpId
			,[source].ApproverId
			,@ModifiedBy
			,@ModifiedDate
			,0
			);

	INSERT INTO dbo.[Log](LogActionID, LogDate, [Description], TrackingValue, UserID)
	VALUES(9, dbo.fnc_GetClientDateTime(), 'Update Personal Flow Setting', @logData, @EmpId)

	DROP TABLE #tblSupervisor;

END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeTransfer_GetLumiGroup] 
AS
/*
Created: 20210901@Quinn.Vo
Task: get Lumi Group
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT [Id],
		[Code],
		[Name]
	FROM LumiHRM.dbo.EmployeeGroup WITH (NOLOCK) 
	WHERE ISNULL(Deleted, 0) = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeTransfer_GetLumiShift] 
AS
/*
Created: 20210901@Quinn.Vo
Task: get Lumi Shift
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT [Id],
		[Code],
		[Name]
	FROM LumiHRM.dbo.[Shift] WITH (NOLOCK) 
	WHERE IsDisabled = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_ApproveAndRejectRequest]
    @WorkFromHomeRequestId INT,    
	@StatusId TINYINT,    
    @ProcessedBy INT,
    @ProcessedDate DATETIME,
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211227@Teddy.le
Task: Approve request employee work from home
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

DECLARE @EmpId int, @Status tinyint, @FromDate datetime, @ToDate datetime
    
SELECT @EmpId = wfhr.EmpId, @Status = wfhr.Status, 
        @FromDate = wfhr.FromDate, @ToDate = wfhr.ToDate
FROM dbo.EmployeeWorkFromHome AS ewfh WITH (NOLOCK)
INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ewfh.WorkFromHomeRequestId
WHERE wfhr.Id = @WorkFromHomeRequestId

set @FromDate = convert(varchar(10),@FromDate,111) 
set @ToDate = convert(varchar(10),@ToDate,111) 

IF (@Status<>1)
BEGIN
    SET @ErrCode = 100
    SET @ErrMsg = 'This request is not pending so it cannot be approved or be rejected'

    RETURN
END

-- if not exist calendar

IF NOT EXISTS(SELECT *
                FROM dbo.EmployeeCalendar WITH (NOLOCK)
                WHERE empid=@empid
                    AND workdate = @fromdate                    
                    )
BEGIN
    SET @ErrCode = 102
    SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'
    RETURN
END

UPDATE dbo.WorkFromHomeRequest
SET [Status] = @StatusId,
    ProcessedBy= @ProcessedBy,
    ProcessedDate= @ProcessedDate,
    ModifiedDate = @ProcessedDate,
    ModifiedBy = @ProcessedBy
WHERE Id = @WorkFromHomeRequestId

--reject
IF (@StatusId = 3)
BEGIN
	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 0
END

EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, @StatusId

-- write log
Declare @LogSPData varchar(4000), @now datetime
set @now=dbo.fnc_GetClientDateTime()

--Log general info
Select @LogSPData = '<EmployeeWorkFromHome_ApproveAndRejectRequest @WorkFromHomeRequestId="'+ coalesce(cast(@WorkFromHomeRequestId as varchar(50)),space(1)) + '" ' +
'StatusId="'+ coalesce(cast(@StatusId as varchar(100)),space(1)) + '" ' +
'FromDate="'+ coalesce(cast(@FromDate as varchar(100)),space(1)) + '" ' +
'ToDate="'+ coalesce(cast(@ToDate as varchar(100)),space(1)) + '" ' +
'ProcessedBy="'+ coalesce(cast(@ProcessedBy as varchar(100)),space(1)) + '" ' +
'ProcessedDate="'+ coalesce(convert(varchar(10),@ProcessedDate, 111 ),space(1)) + '" ' +
'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'    

EXEC dbo.Log_WriteLog
    @LogActionID = 1,
    @LogDate = @now,
    @Description = 'Approve And Reject Employee Work From Home Request',
    @TrackingValue = @LogSPData,
    @UserID = @ProcessedBy

set @ErrCode = 0
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_Delete] @EmployeeWorkFromHomeId INT
	,@DeletedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211213@Teddy.le
Task: Delete Employee Work From Home.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @empId INT, @workFromHomeRequestId INT, @fromDate DATE, @toDate DATE
	SELECT @empId = ewfh.EmpId , @workFromHomeRequestId = ewfh.WorkFromHomeRequestId ,@fromDate = ewfh.FromDate, @toDate = ewfh.ToDate
			FROM dbo.EmployeeWorkFromHome AS ewfh WITH (NOLOCK)
			INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ewfh.WorkFromHomeRequestId
			WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			INNER JOIN dbo.EmployeeWorkFromHome AS ewfh WITH (NOLOCK) ON ewfh.WorkFromHomeRequestId = wfhr.Id
			WHERE Id = @WorkFromHomeRequestId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH does not exists'

		RETURN
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
			INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
			WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId AND wfhr.CreatedBy = @DeletedBy
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Cannot delete employee request'

		RETURN
	END

	DELETE
	FROM dbo.EmployeeWorkFromHome 
	WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId
	
	DELETE
	FROM dbo.WorkFromHomeRequest 
	WHERE Id = @workFromHomeRequestId

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 0

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @fromDate, @toDate, 3

	SELECT @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_DisApproveRequest]
    @WorkFromHomeRequestId INT,       
    @ProcessedBy INT,
    @ProcessedDate DATETIME,
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211228@Teddy.le
Task: Disapprove request employee work from home
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

DECLARE @EmpId int, @Status tinyint, @FromDate datetime, @ToDate datetime
    
SELECT @EmpId = wfhr.EmpId, @Status = wfhr.Status, 
        @FromDate = wfhr.FromDate, @ToDate = wfhr.ToDate
FROM dbo.EmployeeWorkFromHome AS ewfh WITH (NOLOCK)
INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ewfh.WorkFromHomeRequestId
WHERE wfhr.Id = @WorkFromHomeRequestId

set @FromDate = convert(varchar(10),@FromDate,111) 
set @ToDate = convert(varchar(10),@ToDate,111) 

IF (@Status=3)
BEGIN
    SET @ErrCode = 100
    SET @ErrMsg = 'This request was rejected so it could not be rejected again'

    RETURN
END

-- if status is pending
IF (@Status=1)
BEGIN
    SET @ErrCode = 101
    SET @ErrMsg = 'This request is pending so it cannot be disapproved'
    
    RETURN
END

-- if not exist calendar

IF NOT EXISTS(SELECT *
                FROM dbo.EmployeeCalendar WITH (NOLOCK)
                WHERE empid=@empid
                    AND workdate = @fromdate                    
                    )
BEGIN
    SET @ErrCode = 102
    SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'
    RETURN
END

UPDATE dbo.WorkFromHomeRequest
SET [Status] = 1,
    ProcessedBy= @ProcessedBy,
    ProcessedDate= @ProcessedDate,
    ModifiedDate = @ProcessedDate,
    ModifiedBy = @ProcessedBy
WHERE Id = @WorkFromHomeRequestId


EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, 1

-- write log
Declare @LogSPData varchar(4000), @now datetime
set @now=dbo.fnc_GetClientDateTime()

--Log general info
Select @LogSPData = '<EmployeeWorkFromHome_DisApproveRequest @WorkFromHomeRequestId="'+ coalesce(cast(@WorkFromHomeRequestId as varchar(50)),space(1)) + '" ' +
'FromDate="'+ coalesce(cast(@FromDate as varchar(100)),space(1)) + '" ' +
'ToDate="'+ coalesce(cast(@ToDate as varchar(100)),space(1)) + '" ' +
'ProcessedBy="'+ coalesce(cast(@ProcessedBy as varchar(100)),space(1)) + '" ' +
'ProcessedDate="'+ coalesce(convert(varchar(10),@ProcessedDate, 111 ),space(1)) + '" ' +
'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'    

EXEC dbo.Log_WriteLog
    @LogActionID = 1,
    @LogDate = @now,
    @Description = 'DisApprove Employee Work From Home Request',
    @TrackingValue = @LogSPData,
    @UserID = @ProcessedBy

set @ErrCode = 0
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_Get] 
	 @FromDate DATETIME
	,@ToDate DATETIME
	,@EmpId INT = NULL
    ,@GroupId INT = NULL
	,@StatusId INT = NULL
AS
/*
Created: 20211208@Teddy.le
Task: Get Employee Work From Home by date.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT ew.EmployeeWorkFromHomeId
		,ew.FromDate
		,ew.ToDate
		,ew.EmpId
		,e.NickName
		,e.FullName
		,ew.Remark AS HRRemark
		,wfhr.Id AS WorkFromHomeRequestId
		,wfhr.Remark
		,wfhr.Status AS StatusId
		,wfhr.CreatedBy as CreatedById
		,creator.NickName as CreatedBy
		,wfhr.CreatedDate
		,Processor.NickName AS ProcessedBy
		,wfhr.ProcessedDate
	FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
		INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ew.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON wfhr.CreatedBy = creator.Id
		LEFT JOIN dbo.Employee AS Processor WITH (NOLOCK) ON wfhr.ProcessedBy = Processor.Id
	WHERE ((ew.FromDate BETWEEN @FromDate AND @ToDate)
		OR (ew.ToDate BETWEEN @FromDate AND @ToDate)
		OR (@FromDate BETWEEN ew.FromDate AND ew.ToDate)
		OR (@ToDate BETWEEN ew.FromDate AND ew.ToDate)) 
		AND (
            @GroupId IS NULL
            OR e.GroupId = @Groupid
            )
		AND (
            @EmpId IS NULL
            OR e.Id = @EmpId
            )
		AND (
            @StatusId IS NULL
            OR wfhr.Status = @StatusId
            )
	ORDER BY wfhr.CreatedDate DESC
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_GetById] 
	@EmployeeWorkFromHomeId INT
AS
/*
Created: 2021213@Teddy.le
Task: Get Employee Work From Home by Id.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT ew.EmployeeWorkFromHomeId
		,ew.FromDate
		,ew.ToDate
		,ew.EmpId
		,e.NickName
		,e.FullName
		,ew.Remark
	FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
		INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)  ON wfhr.Id = ew.WorkFromHomeRequestId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ew.EmpId = e.Id
	WHERE ew.EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_Insert] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@CreatedBy INT
	,@Remark NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211209@Teddy.le
Task: Insert Employee Work From Home.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @WorkFromHomeRequestId INT
	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
				 INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
			WHERE ew.EmpId = @EmpId
				AND (
					@FromDate BETWEEN ew.FromDate
						AND ew.ToDate
					OR @ToDate BETWEEN ew.FromDate
						AND ew.ToDate
					)
				AND wfhr.Status <> 3
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH is overlapping.'

		RETURN
	END

	INSERT INTO dbo.WorkFromHomeRequest(
		EmpId,
		Status,
		Remark,
		ProcessedDate,
		ProcessedBy,
		FromDate,
		ToDate,
		CreatedDate,
		CreatedBy
	)
	VALUES (
		@EmpId
	   ,2
	   ,@Remark
	   ,dbo.fnc_GetClientDateTime()
	   ,@CreatedBy
	   ,@FromDate
	   ,@ToDate
	   ,dbo.fnc_GetClientDateTime()
	   ,@CreatedBy
		)

	SELECT @WorkFromHomeRequestId = CAST(SCOPE_IDENTITY() AS INT)

	INSERT INTO dbo.EmployeeWorkFromHome (
		 WorkFromHomeRequestId
		,EmpId
		,FromDate
		,ToDate
		,Remark
		,ModifiedBy
		,ModifiedDate
		)
	VALUES (
		@WorkFromHomeRequestId
		,@EmpId
		,@FromDate
		,@ToDate
		,@Remark
		,@CreatedBy
		,dbo.fnc_GetClientDateTime()
		)

	BEGIN
		EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 1 
	END	

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, 2
END
GO

CREATE OR ALTER PROCEDURE [dbo].[EmployeeWorkFromHome_Update] @EmployeeWorkFromHomeId INT
	,@EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@Remark NVARCHAR(500) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211213@Teddy.le
Task: Update Employee Work From Home.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @WorkFromHomeRequestId INT, @FromDateWFH DATE,  @ToDateWFH DATE
	SELECT @WorkFromHomeRequestId = WorkFromHomeRequestId,@FromDateWFH = ew.FromDate, @ToDateWFH = ew.ToDate
			FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
			INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
			WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
				 INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
			WHERE ew.EmployeeWorkFromHomeId <> @EmployeeWorkFromHomeId
				AND ew.EmpId = @EmpId
				AND (
					@FromDate BETWEEN ew.FromDate
						AND ew.ToDate
					OR @ToDate BETWEEN ew.FromDate
						AND ew.ToDate
					)
				AND wfhr.Status <> 3
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH of this employee is overlapping.'

		RETURN
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
			INNER JOIN dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK) ON wfhr.Id = ew.WorkFromHomeRequestId
			WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH of this employee does not exists'

		RETURN
	END

	UPDATE dbo.EmployeeWorkFromHome WITH (ROWLOCK, UPDLOCK)
	SET EmpId = @EmpId
		,FromDate = @FromDate
		,ToDate = @ToDate
		,Remark = @Remark
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = dbo.fnc_GetClientDateTime()
	WHERE EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId

	UPDATE dbo.WorkFromHomeRequest WITH (ROWLOCK, UPDLOCK)
	SET FromDate = @FromDate
		,ToDate = @ToDate
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = dbo.fnc_GetClientDateTime()
	WHERE Id = @WorkFromHomeRequestId

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDateWFH ,@ToDateWFH , 0

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 1 

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDateWFH, @ToDateWFH, 3

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, 1
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ExtensionNO_DELETE_EmployeeInfo] @ExtID INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2014-01-04
Task: Delete Employee Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	IF NOT EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE ExtID = @ExtID
				AND [IsGroup] = 0
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Parameter @ExtID not exists on systems.'

		RETURN
	END
	ELSE
		DELETE
		FROM [dbo].[ExtensionNumbers]
		WHERE ExtID = @ExtID

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ExtensionNO_DELETE_GroupExternal] @ExtID INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2014-01-04
Task: Delete Group Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	IF NOT EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE ExtID = @ExtID
				AND [IsGroup] = 1
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Parameter @ExtID not exists on systems.'

		RETURN
	END

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE GroupID = @ExtID
				AND [IsGroup] = 0
				AND IsInternal = 0
			)
	BEGIN
		SELECT @ErrCode = 101
			,@ErrMsg = N'Team is not empty so cannot delete it'

		RETURN
	END

	DELETE
	FROM [dbo].[ExtensionNumbers]
	WHERE ExtID = @ExtID

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ExtensionNO_GetAll]
AS
/*
Created: Tom.Vo@2014-01-04
Task: Get list Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT *
	FROM (
		SELECT exg.[ExtID]
			,exg.IsInternal AS i1
			,exm_1.IsInternal AS i2
			,exm_2.IsInternal AS i3
			,COALESCE(g.id, exg.[ExtID]) TeamID
			,ISNULL(g.Code, '') Team
			,COALESCE(g.NAME, exg.[FullName]) TeamName
			,COALESCE(ee.Id, exm_1.EmpID, NULL) EmpID
			,COALESCE(ee.FullName, exm_1.FullName, exm_2.FullName) FullName
			,COALESCE(ee.NickName, exm_1.NickName, exm_2.NickName) NickName
			,COALESCE(exm_1.ExtensionNo, exm_2.ExtensionNo) [ExtensionNo]
			,COALESCE(exm_1.ExtID, exm_2.ExtID) EmpExtID
			,exg.IsInternal
			,exg.[OrderNo] [OrderNoGroup]
			,CAST(ROW_NUMBER() OVER (
					ORDER BY exg.[ExtID]
					) AS INT) AS [OrderNoMember]
		FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON exg.GroupID = g.Id
			AND exg.IsInternal = 1
			AND exg.IsGroup = 1
			LEFT JOIN [dbo].[ExtensionNumbers] exm_1 WITH (NOLOCK) ON exg.GroupID = exm_1.GroupID
			AND exg.IsInternal = 1
			AND exm_1.IsGroup = 0
			AND exg.IsGroup = 1
			LEFT JOIN [dbo].Employee ee WITH (NOLOCK) ON exm_1.EmpID = ee.Id
			LEFT JOIN [dbo].[ExtensionNumbers] exm_2 WITH (NOLOCK) ON exg.ExtID = exm_2.GroupID
			AND exg.IsInternal = 0
			AND exm_2.IsGroup = 0
			AND exg.IsGroup = 1
			AND exm_2.IsInternal = 0
		WHERE COALESCE(ee.id, exm_1.EmpID, exm_2.EmpID, - 1) > 0
			OR (
				exg.IsInternal = 0
				AND exg.IsGroup = 1
				)
		) TBL
	WHERE EmpExtID IS NOT NULL
	ORDER BY [OrderNoGroup]
		,[OrderNoMember]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ExtensionNO_GetEmployeeInfo] @isInternal BIT = NULL
	,@EmpExtID INT = NULL
AS
/*
Created: Tom.Vo@2014-01-04
Task: Get list Employee Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	IF @isInternal IS NULL
		SELECT exg.[ExtID]
			,COALESCE(g.Id, exg.[ExtID]) TeamID
			,ISNULL(g.Code, '') Team
			,COALESCE(g.NAME, exg.[FullName]) TeamName
			,COALESCE(ee.Id, exm_1.EmpID, NULL) EmpID
			,COALESCE(ee.FullName, exm_1.FullName, exm_2.FullName) FullName
			,COALESCE(ee.NickName, exm_1.NickName, exm_2.NickName) NickName
			,COALESCE(exm_1.ExtensionNo, exm_2.ExtensionNo) [ExtensionNo]
			,COALESCE(ee.Id, exm_1.ExtID, exm_2.ExtID) EmpExtID
			,exg.IsInternal
			,exg.[OrderNo] [OrderNoGroup]
			,ROW_NUMBER() OVER (
				ORDER BY exg.[ExtID]
				) AS [OrderNoMember]
		FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON exg.GroupID = g.Id
			AND exg.IsInternal = 1
			AND exg.IsGroup = 1
			LEFT JOIN [dbo].[ExtensionNumbers] exm_1 WITH (NOLOCK) ON exg.GroupID = exm_1.GroupID
			AND exg.IsInternal = 1
			AND exm_1.IsGroup = 0
			AND exg.IsGroup = 1
			LEFT JOIN [dbo].Employee ee WITH (NOLOCK) ON exm_1.EmpID = ee.Id
			LEFT JOIN [dbo].[ExtensionNumbers] exm_2 WITH (NOLOCK) ON exg.ExtID = exm_2.GroupID
			AND exg.IsInternal = 0
			AND exm_2.IsGroup = 0
			AND exg.IsGroup = 1
		WHERE COALESCE(ee.Id, exm_1.EmpID, exm_2.EmpID, - 1) > 0
			OR (
				exg.IsInternal = 0
				AND exg.IsGroup = 1
				)
		ORDER BY [OrderNoGroup]
			,[OrderNoMember]
	ELSE IF @EmpExtID IS NULL
		AND @isInternal = 0
		SELECT exg.[ExtID]
			,exg.[ExtID] TeamID
			,'' Team
			,exg.[FullName] TeamName
			,NULL EmpID
			,exm_2.FullName FullName
			,exm_2.NickName NickName
			,exm_2.ExtensionNo [ExtensionNo]
			,exm_2.ExtID EmpExtID
			,exg.IsInternal
			,exg.[OrderNo] [OrderNoGroup]
			,ROW_NUMBER() OVER (
				ORDER BY exg.[ExtID]
				) AS [OrderNoMember]
		FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			LEFT JOIN [dbo].[ExtensionNumbers] exm_2 WITH (NOLOCK) ON exg.ExtID = exm_2.GroupID
			AND exg.IsInternal = 0
			AND exm_2.IsGroup = 0
			AND exg.IsGroup = 1
		WHERE (
				exg.IsInternal = 0
				AND exg.IsGroup = 1
				)
		ORDER BY [OrderNoGroup]
			,[OrderNoMember]
	ELSE IF @EmpExtID IS NULL
		AND @isInternal = 1
		SELECT exg.[ExtID]
			,COALESCE(g.Id, exg.[ExtID]) TeamID
			,ISNULL(g.Code, '') Team
			,COALESCE(g.NAME, exg.[FullName]) TeamName
			,COALESCE(ee.Id, exm_1.EmpID) EmpID
			,COALESCE(ee.FullName, exm_1.FullName) FullName
			,COALESCE(ee.NickName, exm_1.NickName) NickName
			,exm_1.ExtensionNo [ExtensionNo]
			,exm_1.ExtID EmpExtID
			,exg.IsInternal
			,exg.[OrderNo] [OrderNoGroup]
			,CAST(ROW_NUMBER() OVER (
					ORDER BY exg.[ExtID]
					) AS INT) AS [OrderNoMember]
		FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON exg.GroupID = g.Id
			AND exg.IsInternal = 1
			AND exg.IsGroup = 1
			LEFT JOIN [dbo].[ExtensionNumbers] exm_1 WITH (NOLOCK) ON exg.GroupID = exm_1.GroupID
			AND exg.IsInternal = 1
			AND exm_1.IsGroup = 0
			AND exg.IsGroup = 1
			LEFT JOIN [dbo].Employee ee WITH (NOLOCK) ON exm_1.EmpID = ee.Id
		WHERE COALESCE(ee.Id, exm_1.EmpID, - 1) > 0
		ORDER BY [OrderNoGroup]
			,[OrderNoMember]
	ELSE IF @EmpExtID IS NOT NULL
		AND @isInternal = 0
		SELECT exg.[ExtID]
			,exg.[ExtID] TeamID
			,'' Team
			,exg.[FullName] TeamName
			,NULL EmpID
			,exm_2.FullName FullName
			,exm_2.NickName NickName
			,exm_2.ExtensionNo [ExtensionNo]
			,exm_2.ExtID EmpExtID
			,exg.IsInternal
			,exg.[OrderNo] [OrderNoGroup]
			,CAST(ROW_NUMBER() OVER (
					ORDER BY exg.[ExtID]
					) AS INT) AS [OrderNoMember]
		FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			LEFT JOIN [dbo].[ExtensionNumbers] exm_2 WITH (NOLOCK) ON exg.ExtID = exm_2.GroupID
			AND exg.IsInternal = 0
			AND exm_2.IsGroup = 0
			AND exg.IsGroup = 1
		WHERE (
				exg.IsInternal = 0
				AND exg.IsGroup = 1
				)
			AND exm_2.[ExtID] = @EmpExtID
		ORDER BY [OrderNoGroup]
			,[OrderNoMember]
	ELSE IF @EmpExtID IS NOT NULL
		AND @isInternal = 1
		SELECT exg.[ExtID]
			,COALESCE(g.Id, exg.[ExtID]) TeamID
			,ISNULL(g.Code, '') Team
			,COALESCE(g.NAME, exg.[FullName]) TeamName
			,COALESCE(ee.Id, exm_1.EmpID) EmpID
			,COALESCE(ee.FullName, exm_1.FullName) FullName
			,COALESCE(ee.NickName, exm_1.NickName) NickName
			,exm_1.ExtensionNo [ExtensionNo]
			,exm_1.ExtID EmpExtID
			,exg.IsInternal
			,exg.[OrderNo] [OrderNoGroup]
			,CAST(ROW_NUMBER() OVER (
					ORDER BY exg.[ExtID]
					) AS INT) AS [OrderNoMember]
		FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON exg.GroupID = g.Id
			AND exg.IsInternal = 1
			AND exg.IsGroup = 1
			LEFT JOIN [dbo].[ExtensionNumbers] exm_1 WITH (NOLOCK) ON exg.GroupID = exm_1.GroupID
			AND exg.IsInternal = 1
			AND exm_1.IsGroup = 0
			AND exg.IsGroup = 1
			LEFT JOIN [dbo].Employee ee WITH (NOLOCK) ON exm_1.EmpID = ee.Id
		WHERE COALESCE(ee.Id, exm_1.EmpID, - 1) > 0
			AND exm_1.[ExtID] = @EmpExtID
		ORDER BY [OrderNoGroup]
			,[OrderNoMember]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ExtensionNO_GetGroupInfo] @isInternal BIT = NULL
	,@GroupID INT = NULL
AS
/*
Created: Tom.Vo@2014-01-04
Task: Get list group Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	IF @isInternal IS NULL
		SELECT exg.[ExtID]
			,COALESCE(g.Id, exg.[ExtID]) TeamID
			,ISNULL(g.Code, '') Team
			,COALESCE(g.NAME, exg.[FullName]) TeamName
			,SUM(CASE 
					WHEN exm.[ExtID] IS NOT NULL THEN 1
					ELSE 0
					END) TotalMember
			,exg.IsInternal
			,exg.[OrderNo]
		FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON exg.GroupID = g.Id
			AND exg.IsInternal = 1
			AND exg.IsGroup = 1
			LEFT JOIN dbo.ExtensionNumbers exm WITH (NOLOCK) ON exm.IsGroup = 0
			AND (
				(
					exg.GroupID = exm.GroupID
					AND exm.IsInternal = 1
					)
				OR (
					exg.[ExtID] = exm.GroupID
					AND exm.IsInternal = 0
					)
				)
		WHERE exg.IsGroup = 1
		GROUP BY exg.[ExtID]
			,COALESCE(g.Id, exg.[ExtID])
			,ISNULL(g.Code, '')
			,COALESCE(g.NAME, exg.[FullName])
			,exg.IsInternal
			,exg.[OrderNo]
	ELSE IF @GroupID IS NULL
		SELECT exg.[ExtID]
			,COALESCE(g.Id, exg.[ExtID]) TeamID
			,ISNULL(g.Code, '') Team
			,COALESCE(g.NAME, exg.[FullName]) TeamName
			,SUM(CASE 
					WHEN exm.[ExtID] IS NOT NULL THEN 1
					ELSE 0
					END) TotalMember
			,exg.IsInternal
			,exg.[OrderNo]
		FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON exg.GroupID = g.Id
			AND exg.IsInternal = 1
			LEFT JOIN dbo.ExtensionNumbers exm WITH (NOLOCK) ON exm.IsGroup = 0
			AND (
				(
					exg.GroupID = exm.GroupID
					AND exm.IsInternal = 1
					)
				OR (
					exg.[ExtID] = exm.GroupID
					AND exm.IsInternal = 0
					)
				)
		WHERE exg.IsGroup = 1
			AND @isInternal = exg.IsInternal
		GROUP BY exg.[ExtID]
			,COALESCE(g.Id, exg.[ExtID])
			,ISNULL(g.Code, '')
			,COALESCE(g.NAME, exg.[FullName])
			,exg.IsInternal
			,exg.[OrderNo]
	ELSE IF @GroupID IS NOT NULL
		SELECT exg.[ExtID]
			,COALESCE(g.Id, exg.[ExtID]) TeamID
			,ISNULL(g.Code, '') Team
			,COALESCE(g.NAME, exg.[FullName]) TeamName
			,SUM(CASE 
					WHEN exm.[ExtID] IS NOT NULL THEN 1
					ELSE 0
					END) TotalMember
			,exg.IsInternal
			,exg.[OrderNo]
		FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON exg.GroupID = g.Id
			AND exg.IsInternal = 1
			LEFT JOIN dbo.ExtensionNumbers exm WITH (NOLOCK) ON exm.IsGroup = 0
			AND (
				(
					exg.GroupID = exm.GroupID
					AND exm.IsInternal = 1
					)
				OR (
					exg.[ExtID] = exm.GroupID
					AND exm.IsInternal = 0
					)
				)
		WHERE exg.IsGroup = 1
			AND @isInternal = exg.IsInternal
			AND @GroupID = CASE 
				WHEN @isInternal = 1 THEN exg.[GroupID]
				WHEN @isInternal = 0 THEN exg.[ExtID]
				END
		GROUP BY exg.[ExtID]
			,COALESCE(g.Id, exg.[ExtID])
			,ISNULL(g.Code, '')
			,COALESCE(g.NAME, exg.[FullName])
			,exg.IsInternal
			,exg.[OrderNo]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ExtensionNO_Insert_EmployeeInfo] @FullName NVARCHAR(200) = NULL
	,@NickName NVARCHAR(100) = NULL
	,@ExtensionNo NVARCHAR(200)
	,@IsInternal BIT
	,@EmpID INT = NULL
	,@GroupID INT
	,@OrderNo SMALLINT = 1
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2014-01-04
Task: Insert Employee Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @IsGroup BIT = 0

	SET @IsInternal = ISNULL(@IsInternal, 1)

	IF @IsInternal = 1
	BEGIN
		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM dbo.Employee WITH (NOLOCK)
				WHERE Id = @EmpID
				)
		BEGIN
			SELECT @ErrCode = 101
				,@ErrMsg = N'Parameter EmpID not exists on systems.'

			RETURN
		END
	END
	ELSE
	BEGIN
		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
				WHERE ExtID = @GroupID
					AND [IsGroup] = 1
					AND IsInternal = 0
				)
		BEGIN
			SELECT @ErrCode = 103
				,@ErrMsg = N'Parameter EmpID not exists on systems.'

			RETURN
		END
	END

	IF @IsInternal = 1
	BEGIN
		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
				WHERE [GroupID] = @GroupID
					AND [IsInternal] = 1
				)
		BEGIN
			INSERT INTO [ExtensionNumbers] (
				[FullName]
				,[NickName]
				,[ExtensionNo]
				,IsInternal
				,[IsGroup]
				,[EmpID]
				,[GroupID]
				,[OrderNo]
				)
			SELECT N'' [FullName]
				,'' [NickName]
				,'' [ExtensionNo]
				,1 IsInternal
				,1 [IsGroup]
				,NULL [EmpID]
				,@GroupID [GroupID]
				,MAX([OrderNo])
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE [GroupID] <> @GroupID
				AND [IsInternal] = 1
		END

		INSERT INTO [dbo].[ExtensionNumbers] (
			[FullName]
			,[NickName]
			,[ExtensionNo]
			,[IsInternal]
			,[IsGroup]
			,[EmpID]
			,[GroupID]
			,[OrderNo]
			)
		VALUES (
			@FullName
			,@NickName
			,@ExtensionNo
			,@IsInternal
			,@IsGroup
			,@EmpID
			,@GroupID
			,@OrderNo
			)
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[ExtensionNumbers] (
			[FullName]
			,[NickName]
			,[ExtensionNo]
			,[IsInternal]
			,[IsGroup]
			,[EmpID]
			,[GroupID]
			,[OrderNo]
			)
		VALUES (
			@FullName
			,@NickName
			,@ExtensionNo
			,@IsInternal
			,@IsGroup
			,NULL
			,@GroupID
			,@OrderNo
			)
	END

	SET @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ExtensionNO_Insert_GroupExternal] @FullName NVARCHAR(200) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2014-01-04
Task: Insert Employee Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	IF @FullName IS NULL
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Group NAME invalid.'

		RETURN
	END

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE FullName = @FullName
				AND [IsGroup] = 1
				AND IsInternal = 0
			)
	BEGIN
		SELECT @ErrCode = 101
			,@ErrMsg = N'Group NAME exists on systems.'

		RETURN
	END

	DECLARE @GroupID INT

	INSERT INTO [ExtensionNumbers] (
		[FullName]
		,[NickName]
		,[ExtensionNo]
		,IsInternal
		,[IsGroup]
		,[EmpID]
		,[GroupID]
		,[OrderNo]
		)
	SELECT @FullName [FullName]
		,@FullName [NickName]
		,'' [ExtensionNo]
		,0 IsInternal
		,1 [IsGroup]
		,NULL [EmpID]
		,NULL [GroupID]
		,MAX([OrderNo])
	FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
	WHERE [GroupID] <> @GroupID
		AND [IsInternal] = 1

	SET @GroupID = cast(SCOPE_IDENTITY() AS INT)

	UPDATE [dbo].[ExtensionNumbers]
	SET GroupID = @GroupID
	WHERE ExtID = @GroupID

	SET @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ExtensionNO_Update_EmployeeInfo] @ExtID INT
	,@FullName NVARCHAR(200) = NULL
	,@NickName NVARCHAR(100) = NULL
	,@ExtensionNo NVARCHAR(200)
	,@IsInternal BIT
	,@EmpID INT = NULL
	,@GroupID INT
	,@OrderNo SMALLINT = 1
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2014-01-04
Task: Update Employee Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @IsGroup BIT = 0

	IF NOT EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE ExtID = @ExtID
				AND [IsGroup] = 0
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Parameter @ExtID not exists on systems.'

		RETURN
	END

	SET @IsInternal = ISNULL(@IsInternal, 1)

	IF @IsInternal = 1
	BEGIN
		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM dbo.Employee WITH (NOLOCK)
				WHERE id = @EmpID
				)
		BEGIN
			SELECT @ErrCode = 101
				,@ErrMsg = N'Parameter EmpID not exists on systems.'

			RETURN
		END
				--IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[ExtensionNumbers] exg WITH(NOLOCK) WHERE ExtID = @GroupID AND [IsGroup]=1 AND IsInternal = 1)
				--BEGIN
				--    SELECT @ErrCode = 102, @ErrMsg = N'Parameter GroupID not exists on systems.'
				--    RETURN
				--END
	END
	ELSE
	BEGIN
		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
				WHERE ExtID = @GroupID
					AND [IsGroup] = 1
					AND IsInternal = 0
				)
		BEGIN
			SELECT @ErrCode = 103
				,@ErrMsg = N'Parameter EmpID not exists on systems.'

			RETURN
		END
	END

	IF @IsInternal = 1
	BEGIN
		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
				WHERE [GroupID] = @GroupID
					AND [IsInternal] = 1
				)
		BEGIN
			INSERT INTO [ExtensionNumbers] (
				[FullName]
				,[NickName]
				,[ExtensionNo]
				,IsInternal
				,[IsGroup]
				,[EmpID]
				,[GroupID]
				,[OrderNo]
				)
			SELECT N'' [FullName]
				,'' [NickName]
				,'' [ExtensionNo]
				,1 IsInternal
				,1 [IsGroup]
				,NULL [EmpID]
				,@GroupID [GroupID]
				,MAX([OrderNo])
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE [GroupID] <> @GroupID
				AND [IsInternal] = 1
		END
	END

	UPDATE [dbo].[ExtensionNumbers] WITH (ROWLOCK)
	SET [FullName] = @FullName
		,[NickName] = @NickName
		,[ExtensionNo] = @ExtensionNo
		,[IsInternal] = @IsInternal
		,[IsGroup] = 0
		,[EmpID] = CASE 
			WHEN @IsInternal = 1 THEN @EmpID
			ELSE NULL
			END
		,[GroupID] = @GroupID
		,[OrderNo] = @OrderNo
	WHERE ExtID = @ExtID
		AND [IsGroup] = 0

	SET @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ExtensionNO_Update_GroupExternal] @ExtID INT
	,@FullName NVARCHAR(200) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2014-01-04
Task: Update Employee Extensions Number
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @IsGroup BIT = 0

	IF EXISTS (
			SELECT TOP 1 1
			FROM [dbo].[ExtensionNumbers] exg WITH (NOLOCK)
			WHERE FullName = @FullName
				AND [IsGroup] = 1
				AND IsInternal = 0
				AND ExtID <> @ExtID
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Group Name exists on systems.'

		RETURN
	END

	UPDATE [dbo].[ExtensionNumbers] WITH (ROWLOCK)
	SET [FullName] = @FullName
		,[NickName] = [FullName]
	WHERE ExtID = @ExtID
		AND [IsGroup] = 1
		AND IsInternal = 0

	SET @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GroupSupervisor_Get]
AS
/*
Created: 20240423@Lilac.Nguyen
Task: Get group supervisor.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT
		eg.Id AS GroupId,
		eg.Code AS GroupCode,
		eg.[Name] AS GroupName,
		STRING_AGG(e.NickName, ', ') AS Approvers
	FROM dbo.EmployeeGroup AS eg WITH (NOLOCK)
	LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) 
		ON eg.Id = gs.GroupId AND ISNULL(gs.IsDeleted, 0) = 0
	LEFT JOIN dbo.Employee AS e 
		ON e.Id = gs.ApproverId
	WHERE ISNULL(eg.Deleted, 0) = 0
	AND eg.IsSpecialTeam = 0
	GROUP BY eg.Id, eg.Code, eg.[Name];
    
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GroupSupervisor_GetById]
	@GroupId INT
AS
/*
Created: 20240613@Lilac.Nguyen
Task: get approver by groupid.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
	  
	SELECT
		eg.Id AS GroupId,
		eg.[Name] AS GroupName,
		STRING_AGG(e.Id, ', ') AS ApproverIds
	FROM dbo.EmployeeGroup AS eg WITH (NOLOCK)
	LEFT JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) 
		ON eg.Id = gs.GroupId AND ISNULL(gs.IsDeleted, 0) = 0
	LEFT JOIN dbo.Employee AS e 
		ON e.Id = gs.ApproverId
	WHERE ISNULL(eg.Deleted, 0) = 0	AND eg.Id = @GroupId
	GROUP BY eg.Id, eg.Code, eg.[Name];

END
GO

CREATE OR ALTER PROCEDURE [dbo].[GroupSupervisor_Update]
	@GroupId INT
    ,@ApproverIds VARCHAR(255) = NULL
	,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20240423@Lilac.Nguyen
Task: update approver for group.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = dbo.fnc_GetClientDateTime(),  @logData NVARCHAR(MAX);

	SELECT @logData = '<Setting_Old Approver="'+ COALESCE(STRING_AGG(ApproverId, ', '), SPACE(1)) + '" />' +  
	'<Setting_New Approver="'+ COALESCE(CAST(@ApproverIds AS VARCHAR(255)), SPACE(1)) + '" />'
	FROM dbo.GroupSupervisor WITH (NOLOCK)
	WHERE GroupId = @GroupId AND IsDeleted <> 1

	CREATE TABLE #tblSupervisor (ApproverId INT)

	INSERT INTO #tblSupervisor(
            ApproverId
            )
    SELECT val
    FROM dbo.fnc_strSplit(@ApproverIds, ',')

	UPDATE su
	SET su.IsDeleted = 1
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM dbo.GroupSupervisor AS su WITH (ROWLOCK, UPDLOCK)
	WHERE su.GroupId = @GroupId
		AND ISNULL(su.IsDeleted, 0) = 0
		AND NOT EXISTS (
			SELECT 1
			FROM #tblSupervisor AS sp
			WHERE sp.ApproverId = su.ApproverId
			)

	UPDATE su
	SET su.IsDeleted = 0
		,su.ModifiedDate = @ModifiedDate
		,su.ModifiedBy = @ModifiedBy
	FROM dbo.GroupSupervisor AS su WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblSupervisor AS sp ON sp.ApproverId = su.ApproverId
	WHERE su.GroupId = @GroupId
		AND su.IsDeleted = 1

	MERGE dbo.GroupSupervisor AS [target]
	USING #tblSupervisor AS [source] ON (
				[target].GroupId = @GroupId
				AND [target].ApproverId = [source].ApproverId
				)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			GroupId
			,ApproverId
			,ModifiedBy
			,ModifiedDate
			,IsDeleted
			)
		VALUES (
			@GroupId
			,[source].ApproverId
			,@ModifiedBy
			,@ModifiedDate
			,0
			);

	INSERT INTO dbo.[Log](LogActionID, LogDate, [Description], TrackingValue, UserID)
	VALUES (10, dbo.fnc_GetClientDateTime(), 'Update Team Flow Setting', @logData, 0)

	DROP TABLE #tblSupervisor;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[History_Delete]
	@Id INT,
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg VARCHAR(200) = '' OUTPUT
AS

/*
Created: 20240725@Lilac.Nguyen
Task: Delete History
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/

BEGIN
    DELETE dbo.HistoryWork
    WHERE Id = @Id;

	DELETE dbo.HistoryWorkDetail
    WHERE HistoryWorkId = @Id;
         
    IF @@ERROR <> 0
        BEGIN                 
            SELECT @ErrCode = 1,
                @ErrMsg = N'Delete History failed.';
        END;
    ELSE
        BEGIN                 
            SELECT @ErrCode = 0, @ErrMsg = '';
        END;         
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[History_Get] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@GroupId INT = NULL
	,@EmpId INT = NULL
AS
/*
Created: 20240723@Lilac.Nguyen
Task: Get history.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT h.Id
	,h.EmpId
	,e.NickName
	,e.FullName
	,eg.[Name] AS GroupName
	,jt.[Name] AS JobTitleName
	,jg.[Name] AS JobGradeName
	,h.AffectedDate
	FROM dbo.HistoryWork AS h WITH (NOLOCK)
		INNER JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = h.EmpId
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON eg.Id = hd.GroupId
		LEFT JOIN HRM.dbo.JobGrade AS jg WITH (NOLOCK) ON jg.JobGradeId = hd.JobGradeId
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON jt.JobTitleId = hd.JobTitleId
	WHERE (@GroupId IS NULL OR hd.GroupId = @GroupId)
		AND (@EmpId IS NULL OR h.EmpId = @EmpId)
		AND DATEDIFF(DAY, @FromDate, h.AffectedDate) >= 0
		AND DATEDIFF(DAY, @ToDate, h.AffectedDate) <= 0
       
	ORDER BY h.AffectedDate DESC
            
END
GO

CREATE OR ALTER PROCEDURE [dbo].[History_GetByEmpId]
    @EmpId INT
AS
/*
Created: 20240729@Lilac.Nguyen
Task: Get History by EmpId.
DB: NexHRM
Original: 
Revisions: 
	- 20250319@Henry.Dinh: Order by AffectedDate desc
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT h.Id
	,eg.[Name] AS GroupName
	,jt.[Name] AS JobTitleName
	,jg.[Name] AS JobGradeName
	,mg.[Name] AS ManagementGradeName
	,h.AffectedDate
    FROM dbo.HistoryWork AS h WITH (NOLOCK)
		INNER JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = h.EmpId
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON eg.Id = hd.GroupId
		LEFT JOIN HRM.dbo.JobGrade AS jg WITH (NOLOCK) ON jg.JobGradeId = hd.JobGradeId
		LEFT JOIN HRM.dbo.ManagementGrade AS mg WITH (NOLOCK) ON mg.ManagementGradeId = hd.ManagementGradeId
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON jt.JobTitleId = hd.JobTitleId
    WHERE h.EmpId = @EmpId
	ORDER BY h.AffectedDate DESC  
END
GO

CREATE OR ALTER PROCEDURE [dbo].[History_GetById]
    @Id INT
AS
/*
Created: 20240724@Lilac.Nguyen
Task: Get History by Id.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT h.Id
	,h.EmpId AS EmployeeId
	,e.NickName
	,e.FullName
	,hd.GroupId
	,hd.JobTitleId
	,hd.JobGradeId
	,h.AffectedDate
    FROM dbo.HistoryWork AS h WITH (NOLOCK)
		INNER JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = h.EmpId
    WHERE h.Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[History_Insert] 
    @EmpId INT
    ,@GroupId INT = NULL
	,@JobTitleId INT = NULL
	,@JobGradeId INT = NULL
    ,@AffectedDate DATETIME
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240724@Lilac.Nguyen
Task: Insert History
DB: NexHRM
Original: 

Revisions:
	- Remove check different from the current value for user init data
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
	DECLARE @NewId INT;

	--DECLARE @CurrentGroupId INT, @CurrentJobTitleId INT, @CurrentJobGradeId INT;

	--SELECT 
	--	@CurrentGroupId = GroupId,
	--	@CurrentJobTitleId = JobTitleId
	--FROM dbo.Employee WITH (NOLOCK)
	--WHERE Id = @EmpId;

	--SELECT @CurrentJobGradeId = JobGradeId
	--FROM HRM.dbo.EmployeeJobGrade WITH (NOLOCK)
	--WHERE EmpId = @EmpId AND IsCurrentUsed = 1

	--IF @GroupId = @CurrentGroupId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Team must be different from the current Team!';
	--	RETURN;
	--END

	--IF @JobTitleId = @CurrentJobTitleId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Job Title must be different from the current Job Title!';
	--	RETURN;
	--END

	--IF @JobGradeId = @CurrentJobGradeId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Job Grade must be different from the current Job Grade!';
	--	RETURN;
	--END

    INSERT INTO dbo.HistoryWork(
		EmpId
		,AffectedDate
		,ModifiedBy
		,ModifiedDate
        )
    VALUES (
        @EmpId
		,@AffectedDate
		,@ModifiedBy
		,dbo.fnc_GetClientDateTime()
		)

	SET @NewId = SCOPE_IDENTITY();

	INSERT INTO dbo.HistoryWorkDetail(
		HistoryWorkId
		,GroupId
		,JobTitleId
		,JobGradeId
        )
    VALUES (
        @NewId
		,@GroupId
		,@JobTitleId
		,@JobGradeId
		)

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Insert history failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0 ,@ErrMsg = ''
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[History_Update] 
	@Id INT
	,@EmpId	INT
    ,@GroupId INT = NULL
	,@JobTitleId INT = NULL
	,@JobGradeId INT = NULL
    ,@AffectedDate DATETIME
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240724@Lilac.Nguyen
Task: Update a history
DB: NexHRM
Original: 

Revisions:  
	- Remove check different from the current value for user init data
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;
	--DECLARE @CurrentGroupId INT, @CurrentJobTitleId INT, @CurrentJobGradeId INT;

	--SELECT 
	--	@CurrentGroupId = GroupId,
	--	@CurrentJobTitleId = JobTitleId
	--FROM dbo.Employee WITH (NOLOCK)
	--WHERE Id = @EmpId;

	--SELECT @CurrentJobGradeId = JobGradeId
	--FROM HRM.dbo.EmployeeJobGrade WITH (NOLOCK)
	--WHERE EmpId = @EmpId AND IsCurrentUsed = 1

	--IF @GroupId = @CurrentGroupId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Team must be different from the current Team!';
	--	RETURN;
	--END

	--IF @JobTitleId = @CurrentJobTitleId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Job Title must be different from the current Job Title!';
	--	RETURN;
	--END

	--IF @JobGradeId = @CurrentJobGradeId
	--BEGIN
	--	SET @ErrCode = 1;
	--	SET @ErrMsg = 'New Job Grade must be different from the current Job Grade!';
	--	RETURN;
	--END

    UPDATE dbo.HistoryWork WITH (ROWLOCK, UPDLOCK)
    SET 
		AffectedDate = @AffectedDate
        ,ModifiedBy = @ModifiedBy
        ,ModifiedDate = dbo.fnc_GetClientDateTime()
    WHERE Id = @Id

	UPDATE dbo.HistoryWorkDetail WITH (ROWLOCK, UPDLOCK)
    SET 
		GroupId = @GroupId
        ,JobTitleId = @JobTitleId
        ,JobGradeId = @JobGradeId
    WHERE HistoryWorkId = @Id
  
    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update history failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0 ,@ErrMsg = ''
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Holiday_Delete] @Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/20@Dan
Task: delete a holiday
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    DECLARE @Fromdate DATETIME
        ,@todate DATETIME

    SELECT @fromdate = fromdate
        ,@todate = todate
    FROM dbo.Holiday WITH (NOLOCK)
    WHERE Id = @id

    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- update working calendar for employee tu turn off the isHoliday 
        UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
        SET IsHoliday = 0
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE WorkDate BETWEEN @FromDate
                AND @ToDate

        -- update the attendance data
        EXEC EmployeeAttendance_ProcessingAndInsert NULL
            ,@fromdate
            ,@todate
            ,1

        -- delete the holiday
        DELETE Holiday
        WHERE Id = @id

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Delete holiday unsuccessfully.'
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Holiday_GetById] @Id INT
AS
/*
Created: 20160224@Quinn.Vo
Task: Get holiday by id.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    SELECT Id
        ,HolidayName AS NAME
        ,FromDate
        ,ToDate
    FROM HOLIDAY WITH (NOLOCK)
    WHERE Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Holiday_GetByYear] @Year INT
AS
/*
Created: 20160224@Quinn.Vo
Task: Get holiday by year.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    SELECT Id
        ,HolidayName AS NAME
        ,FromDate
        ,ToDate
    FROM HOLIDAY WITH (NOLOCK)
    WHERE YEAR(FromDate) = @Year
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Holiday_Insert] @HolidayName NVARCHAR(50)
    ,@FromDate DATETIME
    ,@ToDate DATETIME
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*  
Created: 2012/04/20@Dan  
Task: insert a holiday  
DB: NexHR  
Original:   
  
Revisions:      
Param's Explanation:  
  
*/
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.Holiday WITH (NOLOCK)
                WHERE (
                        FromDate BETWEEN @FromDate
                            AND @ToDate
                        )
                    OR (
                        @FromDate BETWEEN FromDate
                            AND ToDate
                        )
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'There are same holiday in date range.'

        RETURN
    END

    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Holiday] (
            HolidayName
            ,FromDate
            ,ToDate
            )
        VALUES (
            @HolidayName
            ,@FromDate
            ,@ToDate
            )

        DECLARE @id INT

        IF @@ERROR <> 0
            SET @ID = 0
        ELSE
            SET @id = cast(SCOPE_IDENTITY() AS INT)

        -- update working calendar for employee  
        UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
        SET IsHoliday = 1
			,ShiftId = NULL
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE WorkDate BETWEEN @FromDate
                AND @ToDate

        -- update attendate record 
        EXEC EmployeeAttendance_ProcessingAndInsert NULL
            ,@fromdate
            ,@todate
            ,1

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Create holiday unsuccessfully.'
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Holiday_Update] @Id INT
    ,@HolidayName NVARCHAR(50)
    ,@FromDate DATETIME
    ,@ToDate DATETIME
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/20@Dan
Task: update a holiday
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

    SET XACT_ABORT ON;

    DECLARE @OldFromdate DATETIME
        ,@Oldtodate DATETIME

    SELECT @OldFromdate = fromdate
        ,@Oldtodate = todate
    FROM dbo.Holiday WITH (NOLOCK)
    WHERE Id = @id

    BEGIN TRY
        BEGIN TRANSACTION;

        IF (
                @OldFromdate = @FromDate
                AND @ToDate = @Oldtodate
                )
        BEGIN
            UPDATE dbo.Holiday WITH (ROWLOCK, UPDLOCK)
            SET HolidayName = @HolidayName
            WHERE ID = @ID
        END
        ELSE
        BEGIN
            -- update working calendar for employee tu turn off the isHoliday for previous  time range
            UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
            SET IsHoliday = 0
            WHERE WorkDate BETWEEN @OldFromdate
                    AND @Oldtodate

            -- just update attendate record from past to today
            EXEC EmployeeAttendance_ProcessingAndInsert NULL
                ,@OldFromdate
                ,@Oldtodate
                ,1

            -- update working calendar for employee tu turn on the isHoliday  for new range
            UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
            SET IsHoliday = 1
				,ShiftId = NULL
                ,ModifiedBy = @ModifiedBy
                ,ModifiedDate = dbo.fnc_GetClientDateTime()
            WHERE WorkDate BETWEEN @FromDate
                    AND @ToDate

            -- update attendate record 
            EXEC EmployeeAttendance_ProcessingAndInsert NULL
                ,@fromdate
                ,@todate
                ,1

            UPDATE dbo.Holiday WITH (ROWLOCK, UPDLOCK)
            SET HolidayName = @HolidayName
                ,FromDate = @FromDate
                ,ToDate = @ToDate
            WHERE ID = @ID
        END

        COMMIT TRANSACTION;

        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SELECT @ErrCode = 1
            ,@ErrMsg = N'Create holiday unsuccessfully.'
    END CATCH;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[HRM_GetRedmineUsers]
AS
/*
Created: 20180806@Oliver.Dang
Task: Get Bio Device.
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.Id AS EmpId
		  ,e.FullName
		  ,e.NickName
		  ,r.Id AS RedmineId
		  ,ec.ConversationId
	FROM [dbo].[EmployeeSetting] AS s WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON s.EmpId = e.Id
		INNER JOIN [RedmineNexcel].[dbo].[Users] AS r WITH (NOLOCK) ON r.[Login] = REPLACE(e.DomainUsername, 'spvn\','')
		-- LEFT JOIN [NexBot].[dbo].[ConversationUser] AS c WITH (NOLOCK) ON e.NickName = c.NickName
		LEFT JOIN dbo.EmployeeConversation AS ec ON e.Id = ec.EmpId
	WHERE e.[Enabled] = 1 
		AND e.isDeleted = 0
		AND s.DisplayRedmineTask = 1
END
GO

CREATE OR ALTER PROCEDURE [dbo].[HRM_SyncData_GroupReport_GetEmployeeInfo] @LastModifiedDate DATETIME = NULL
AS
/*
Created: 20250220@Tim
Task: get employee info
DB: NexHRM
Original:

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT e.Id AS EmpId
	    ,dept.Code AS DepartmentCode
	    ,dept.[Name] AS DepartmentName
	    ,team.Code AS TeamCode
	    ,team.[Name] AS TeamName
		,e.Code
		,e.NickName
		,e.FullName
        ,e.DOB
        ,e.Gender
        ,jl.[Name] AS JobLevel
	    ,jt.[Name] AS JobTitle
        ,e.GroupDate AS JoinedDate
		,e.JoinedDate AS CompanyDate
		,e.ConfirmedDate
		,e.ResignedDate
        ,CASE WHEN team.[Name] = 'Expat' THEN 0 ELSE 1 END AS IsLocal
		,e.[Enabled]
		,e.IsDeleted
		,e.IsSpecialUser
		,e.ModifiedDate 
	FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeGroup AS team WITH (NOLOCK) ON e.GroupId = team.Id
        INNER JOIN dbo.EmployeeExt AS ext WITH (NOLOCK) ON e.Id = ext.Id
        LEFT JOIN HRM.dbo.EmployeeJobGrade AS ejl WITH (NOLOCK) ON e.Id = ejl.EmpId
        AND ejl.IsCurrentUsed = 1
        LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON team.DepartmentId = dept.DepartmentId
        LEFT JOIN HRM.dbo.JobGrade AS jl WITH (NOLOCK) ON ejl.JobGradeId = jl.JobGradeId
        LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
	WHERE @LastModifiedDate IS NULL
        OR e.ModifiedDate > @LastModifiedDate
        OR team.ModifiedDate > @LastModifiedDate
        OR dept.ModifiedDate > @LastModifiedDate
        OR ejl.ModifiedDate > @LastModifiedDate
        OR jl.ModifiedDate > @LastModifiedDate
        OR jt.ModifiedDate > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[JobTitle_Get] @JobTitle NVARCHAR(100) = ''
AS
/*
Created: 20160224@Quinn.Vo
Task: Get job title data.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    IF @JobTitle IS NULL
        SET @JobTitle = ''

	SELECT jt.JobTitleId AS Id
		,jt.Name
		,COUNT(e.Id) AS NumberOfEmployee
	FROM HRM.dbo.JobTitle AS jt WITH (NOLOCK)
	LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON jt.JobTitleId = e.JobTitleId
	WHERE jt.Name LIKE '%' + @JobTitle + '%'
	AND ISNULL(e.IsDeleted, 0) = 0
	GROUP BY jt.JobTitleId
        ,jt.[Name]
    ORDER BY jt.[Name]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_Delete] @Id      INT,
                                              @ErrCode INT          = 0 OUTPUT,
                                              @ErrMsg  VARCHAR(200) = '' OUTPUT
AS
     BEGIN
         -- SET NOCOUNT ON added to prevent extra result sets from
         -- interfering with SELECT statements.
         SET NOCOUNT ON;
         IF(EXISTS
           (
               SELECT 1
               FROM dbo.LateInEarlyOut lieo
               WHERE lieo.Id = @Id
           ))
             BEGIN
                 DELETE dbo.LateInEarlyOut
                 WHERE dbo.LateInEarlyOut.Id = @Id;
                 IF @@ERROR <> 0
                     BEGIN
                         SELECT @ErrCode = 1,
                                @ErrMsg = N'Delete Late in/Early out failed.';
                     END;
                 ELSE
                     BEGIN
                         SELECT @ErrCode = 0,
                                @ErrMsg = '';
                     END;
             END;
         ELSE
             BEGIN
                 SELECT @ErrCode = 1,
                        @ErrMsg = N'Can not find this request.';
             END;
     END;

GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetAllByEmpId] @EmpId INT,
                                                     @Year  INT
AS
     BEGIN
         SELECT lieo.Id,
                e.NickName,
                (CASE lieo.Type
                     WHEN 1
                     THEN 'Late in'
                     WHEN 2
                     THEN 'Early out'
                 END) 'Types',
                lieo.WorkDate,
                lieo.Minutes,
                lieo.ModifiedBy,
                lieo.ModifiedDate
         FROM dbo.LateInEarlyOut lieo
              JOIN dbo.Employee e ON lieo.EmpId = e.Id
         WHERE lieo.EmpId = @EmpId
               AND YEAR(lieo.WorkDate) = @Year
         ORDER BY lieo.WorkDate DESC;
     END;
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetAllEmployee] @Date DATETIME
AS
/*
Created: 20160321@HarveyNguyen
Task: Show dashboard
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.Id 'EmpId'
		,e.NickName
		,CASE 
			WHEN lieo.Type = 1 THEN 'Late in'
			ELSE 'Early out'
			END AS 'Types'
		,lieo.Minutes
	FROM dbo.LateInEarlyOut lieo WITH (NOLOCK)
		JOIN dbo.Employee e WITH (NOLOCK) ON e.Id = lieo.EmpId
	WHERE lieo.WorkDate = @Date
	ORDER BY e.NickName
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetById] @Id INT
AS
BEGIN
    SELECT lieo.Id,
                e.NickName,
                lieo.Type,
                lieo.WorkDate,
                lieo.Minutes,
                lieo.ModifiedBy,
                lieo.ModifiedDate
         FROM dbo.LateInEarlyOut lieo
              JOIN dbo.Employee e ON lieo.EmpId = e.Id
         WHERE lieo.Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutDisciplineViolate] 
	@FromDate DATE
    ,@ToDate DATE
AS
/*
    Created: 20200709@Merlin.Le
    Task: Get late in/ early out violate disciplines
    DB: NexHRM
    Revisions: 
		20250507@Henry.Dinh: update group id from 11 to 1081
    Original: 

*/
BEGIN
    SET NOCOUNT ON;

    IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
    BEGIN
        DROP TABLE #LateInEarlyOutSummary;
    END;

    CREATE TABLE #LateInEarlyOutSummary (
        EmpId INT
        ,[Month] INT
        ,[Year] INT
        ,TotalTimes INT
        );

    INSERT INTO #LateInEarlyOutSummary
    SELECT DISTINCT tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
        ,COUNT(*)
    FROM [dbo].LateInEarlyOutTrans AS tr WITH (NOLOCK)
        INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
        AND e.GroupId NOT IN (40, 1081) -- expat, core
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
    WHERE tr.[Type] IN (1, 2)
        AND (CAST(tr.WorkDate AS DATE) BETWEEN @FromDate
            AND @ToDate)
       
    GROUP BY tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
    HAVING COUNT(*) >= 4

    SELECT COUNT(tr.TotalTimes) AS TotalTimes
        ,e.Id AS EmpId
        ,e.NickName
        ,e.FullName
        ,e.GroupId
        ,g.Code
        ,g.[Name]
    FROM [dbo].Employee AS e WITH (NOLOCK)
        INNER JOIN [dbo].EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
        LEFT JOIN #LateInEarlyOutSummary AS tr ON e.Id = tr.EmpId
    WHERE e.GroupId NOT IN (40, 1081) -- expat, core
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
        AND  (e.Id IN (
                    SELECT DISTINCT EmpId
                    FROM #LateInEarlyOutSummary
                    )
				)
		
    GROUP BY e.Id 
        ,e.NickName
        ,e.FullName
        ,e.Code
		,e.Roles
		,e.GroupDate
        ,e.GroupId
        ,g.Code
        ,g.[Name]
        ,g.[Rank]

	HAVING COUNT(tr.TotalTimes) >=3
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate;

    SELECT su.EmpId
        ,CAST(CAST([Year] AS VARCHAR) + '-' + CAST([Month] AS VARCHAR) + '-' + CAST(1 AS VARCHAR) AS DATE) AS [Month]
        ,TotalTimes
    FROM #LateInEarlyOutSummary AS su WITH (NOLOCK)
	INNER JOIN (
		SELECT EmpId
		FROM #LateInEarlyOutSummary
		GROUP BY EmpId
		HAVING COUNT(EmpId) >=3) AS tmp ON tmp.EmpId = su.EmpId
    ORDER BY su.EmpId
        ,[Year]
        ,[Month]

    DROP TABLE #LateInEarlyOutSummary;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutOnMonth] 
	@Year INT
	,@Month INT
	
AS
/*
	Created: 20200708@Merlin.Le
	Task: Get late in/ early out on current month
	DB: NexHRM & NexBot
	Original: 

	Revisions:  
		20250507@Henry.Dinh: update group id from 11 to 1081
	Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary
	END

	CREATE TABLE #LateInEarlyOutSummary(
		EmpId INT
		,MinuteOnMonth INT
		,TimeOnMonth INT)

	INSERT INTO #LateInEarlyOutSummary(EmpId, MinuteOnMonth, TimeOnMonth)

	SELECT EmpId
		,SUM(CASE 
				WHEN [Type] <> 0 THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0
						THEN 1
				ELSE 0
				END)
	FROM [dbo].LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE e.GroupId NOT IN (1081,40)
	 AND YEAR(WorkDate) = @Year AND Month(WorkDate) = @Month
	GROUP BY EmpId

	SELECT e.Id AS EmpId
		  ,e.FullName
		  ,e.NickName
		  ,ec.ConversationId
		  ,ls.MinuteOnMonth
		  ,ls.TimeOnMonth
	FROM #LateInEarlyOutSummary AS ls WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON ls.EmpId = e.Id
		INNER JOIN [dbo].[EmployeeGroup] AS g WITH (NOLOCK) ON e.GroupId = g.Id
		--LEFT JOIN [NexBot].[dbo].[ConversationUser] AS c WITH (NOLOCK) ON e.NickName = c.NickName
		LEFT JOIN [dbo].[EmployeeConversation] AS ec WITH (NOLOCK) ON ec.EmpId = e.Id
	WHERE e.isDeleted = 0 AND e.[Enabled] = 1
		AND ls.TimeOnMonth >= 3
		
	DROP TABLE #LateInEarlyOutSummary	
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_GetLateInEarlyOutToDeduct] @Year INT	
AS
/*
Created: 20200709@Merlin.Le
Task: Get late in/early out need to deduct
DB: NexHRM
Original: 
Revisions:
	- 20250328@Henry.Dinh: Update GroupID 11-40 -> GroupCode MGT/COR/EXP
*/
BEGIN
	SET NOCOUNT ON;
	IF OBJECT_ID('#tblAnnualLeaveBalance') IS NOT NULL
	BEGIN
		DROP TABLE #tblAnnualLeaveBalance;
	END;

	CREATE TABLE #tblAnnualLeaveBalance (
		EmpId INT
		,AnnualLeaveBalance NUMERIC(10, 1)
		);

	IF OBJECT_ID('#PendingAnnualLeaveRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingAnnualLeaveRequest;
	END;

	CREATE TABLE #PendingAnnualLeaveRequest (
		EmpId INT
		,TotalDays NUMERIC(10, 1)
		);

	IF OBJECT_ID('#PreviousYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #PreviousYearBalance;
	END;

	CREATE TABLE #PreviousYearBalance (
		EmpId INT
		,[Minutes] INT
		,ConversionValue INT
		);

	IF OBJECT_ID('#CurrentYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #CurrentYearBalance;
	END;

	CREATE TABLE #CurrentYearBalance (
		EmpId INT
		,[Minutes] INT
		,Overtime INT
		);

	IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary;
	END;

	CREATE TABLE #LateInEarlyOutSummary (
		EmpId INT
		,TotalMinutes INT
		,TotalTimes INT
		,TotalMinutesLeaveDays INT
		);

	INSERT INTO #PreviousYearBalance (
		EmpId
		,[Minutes]
		,ConversionValue
		)
	SELECT e.Id
		,(
			ISNULL(la.[Minutes], 0) - CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
		,CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0)
			END
	FROM [dbo].Employee AS e WITH (NOLOCK)
		LEFT JOIN [dbo].LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = (@Year - 1)
		LEFT JOIN [dbo].OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = (@Year - 1)
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0

	INSERT INTO #CurrentYearBalance (
		EmpId
		,[Minutes]
		,Overtime
		)
	SELECT e.Id
		,CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN (ISNULL(la.[Minutes], 0) - ISNULL(oty.[Minutes], 0))
			ELSE 0
			END
		,(CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
	FROM [dbo].Employee AS e WITH (NOLOCK)
		LEFT JOIN [dbo].LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = @Year
		LEFT JOIN #PreviousYearBalance AS prev ON e.Id = prev.EmpId
		LEFT JOIN [dbo].OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = @Year
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0

	INSERT INTO #LateInEarlyOutSummary (
		EmpId
		,TotalMinutes
		,TotalTimes
		,TotalMinutesLeaveDays
		)
	SELECT EmpId
		,SUM(CASE 
				WHEN [Type] <> 0 THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0 THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] = 0 THEN [Minutes]
				ELSE 0
				END)
	FROM [dbo].LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE e.IsSpecialUser = 0
		AND YEAR(WorkDate) = @Year
	GROUP BY EmpId;

	INSERT INTO #tblAnnualLeaveBalance(EmpId, AnnualLeaveBalance)
	SELECT lt.EmpId, SUM(lt.CreditDays) - SUM(lt.DeductDays)
	FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON lt.EmpId = e.Id
	WHERE e.Enabled = 1 
	AND e.IsDeleted = 0
	AND YEAR(lt.FromDate) = @Year
	AND lt.Deleted = 0
	GROUP BY lt.EmpId

	INSERT INTO #PendingAnnualLeaveRequest (
		EmpId
		,TotalDays
		)
	SELECT e.Id, SUM(ISNULL(wk.TotalDays, 0))
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON e.Id = wk.EmpId
	WHERE wk.RequestType = 6
		AND wk.[Status] = 1
		AND wk.LeaveTypeID IN (2, 30)
		AND YEAR(wk.FromDate) = @Year
	GROUP BY e.Id


	SELECT dt.TotalMinutes AS TotalCurrentYear
		,dt.TotalTimes AS TotalTimesCurrentYear
		,dt.TotalMinutesLeaveDays AS LeaveDeduction
		,ISNULL(pv.[Minutes], 0) AS BalanceLastYear
		,ISNULL(cu.[Minutes], 0) AS BalanceCurrentYear
		,ISNULL(cu.Overtime, 0) AS OvertimeBalance
		,pv.EmpId
		,e.NickName
		,e.FullName
		,al.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) AS AnnualLeaveBalance
	FROM #PreviousYearBalance AS pv
		INNER JOIN #CurrentYearBalance AS cu ON pv.EmpId = cu.EmpId
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON pv.EmpId = e.Id
		INNER JOIN [dbo].EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
		INNER JOIN (
			SELECT EmpId
			,(SUM(CreditDays) - SUM(DeductDays)) AS AnnualLeaveBalance
			FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e ON lt.EmpId = e.Id AND e.isDeleted = 0
			WHERE YEAR(FromDate) = @Year
			AND ISNULL(lt.Deleted, 0) = 0
			GROUP BY EmpId
		) AS leasum ON e.Id = leasum.EmpId
		LEFT JOIN #LateInEarlyOutSummary AS dt ON pv.EmpId = dt.EmpId
		LEFT JOIN #tblAnnualLeaveBalance AS al ON al.EmpId = pv.EmpId
		LEFT JOIN #PendingAnnualLeaveRequest AS rq ON e.Id = rq.EmpId
	WHERE eg.Code NOT IN ('MGT', 'COR', 'EXP')
		AND ISNULL(cu.[Minutes], 0) >= 240
		AND (leasum.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) > 0)
		
	ORDER BY e.NickName;

	DROP TABLE #PreviousYearBalance;

	DROP TABLE #CurrentYearBalance;

	DROP TABLE #LateInEarlyOutSummary;

	DROP TABLE #PendingAnnualLeaveRequest;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_Insert] @WorkDate DATETIME
    ,@Type TINYINT
    ,@Minutes TINYINT
    ,@EmpId INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF (
            EXISTS (
                SELECT 1
                FROM dbo.LateInEarlyOut lieo
                WHERE lieo.WorkDate = @WorkDate
                    AND lieo.Type = @Type
                    AND lieo.EmpId = @EmpId
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = 'This request has already existed !';

        RETURN;
    END;

    INSERT INTO dbo.LateInEarlyOut (
        --Id - this column value is auto-generated
        WorkDate
        ,[Type]
        ,[Minutes]
        ,EmpId
        ,ModifiedBy
        ,ModifiedDate
        )
    VALUES (
        @WorkDate
        ,@Type
        ,--1: LateIn - 2: EarlyOut
        @Minutes
        ,@EmpId
        ,@ModifiedBy
        ,dbo.fnc_GetClientDateTime()
        );

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Create Late in/Early out failed.';
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = '';
    END;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_InsertTransaction] 
    @EmpId INT
    ,@WorkDate DATE
    ,@LateIn INT
    ,@EarlyOut INT
AS
/*
Created: 20150805@Quinn.Vo
Task: Insert late in/early out transaction.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    DECLARE @OldLateIn INT, @OldEarlyOut INT, @MinutesPreviousYear INT

    IF(@LateIn > 60 OR @EarlyOut > 60)
    BEGIN
        SELECT @LateIn = 0, @EarlyOut = 0
    END
    ELSE
    BEGIN
        IF (@LateIn > 0)
        BEGIN
            IF(@LateIn <= 30)
            BEGIN
                SET @LateIn = 30
            END
            ELSE
            BEGIN 
                SET @LateIn = 60
            END
        END

        IF (@EarlyOut > 0)
        BEGIN
            IF(@EarlyOut <= 30)
            BEGIN
                SET @EarlyOut = 30
            END
            ELSE
            BEGIN 
                SET @EarlyOut = 60
            END
        END
    END

    SELECT @OldLateIn = [Minutes]
    FROM dbo.LateInEarlyOutTrans WITH (NOLOCK)
    WHERE EmpId = @EmpId 
    AND DATEDIFF(DD, WorkDate, @WorkDate) = 0
    AND [Type] = 1
	AND ISNULL(IsManual, 0) = 0

    SELECT @OldEarlyOut = [Minutes]
    FROM dbo.LateInEarlyOutTrans WITH (NOLOCK)
    WHERE EmpId = @EmpId 
    AND DATEDIFF(DD, WorkDate, @WorkDate) = 0
    AND [Type] = 2
	AND ISNULL(IsManual, 0) = 0

    SELECT @OldLateIn = ISNULL(@OldLateIn, 0), @OldEarlyOut = ISNULL(@OldEarlyOut, 0)
    IF(ISNULL(@OldLateIn, 0) <> ISNULL(@LateIn, 0) OR ISNULL(@OldEarlyOut, 0) <> ISNULL(@EarlyOut, 0))
    BEGIN
        IF(ISNULL(@OldLateIn, 0) <> ISNULL(@LateIn, 0))
        BEGIN
            IF(EXISTS (SELECT 1 FROM dbo.LateInEarlyOutYearly WITH (NOLOCK) WHERE EmpId = @EmpId AND [Year] = YEAR(@WorkDate)))
            BEGIN
                UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
                SET [Minutes] = [Minutes] - @OldLateIn + @LateIn, ModifiedDate = dbo.fnc_GetClientDateTime()
                WHERE EmpId = @EmpId AND [Year] >= YEAR(@WorkDate)
            END
            ELSE
            BEGIN
                SELECT @MinutesPreviousYear = [Minutes]
                FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
                WHERE EmpId = @EmpId 
                    AND [Year] = YEAR(@WorkDate) - 1

                INSERT INTO dbo.LateInEarlyOutYearly (EmpId, [Year], [Minutes], ModifiedDate)
                VALUES (@EmpId, YEAR(@WorkDate), (ISNULL(@MinutesPreviousYear, 0) - @OldLateIn + @LateIn), dbo.fnc_GetClientDateTime())
            END

            DELETE FROM dbo.LateInEarlyOutTrans 
			WHERE EmpId = @EmpId 
			AND WorkDate = @WorkDate 
			AND [Type] = 1
			AND ISNULL(IsManual, 0) = 0
            
            IF(@LateIn <> 0)
            BEGIN
                INSERT INTO dbo.LateInEarlyOutTrans (EmpId, WorkDate, [Type], [Minutes], ModifiedDate)
                VALUES (@EmpId, @WorkDate, 1, @LateIn, dbo.fnc_GetClientDateTime())
            END
        END
        
        IF(ISNULL(@OldEarlyOut, 0) <> ISNULL(@EarlyOut, 0))
        BEGIN
            IF(EXISTS (SELECT 1 FROM dbo.LateInEarlyOutYearly WITH (NOLOCK) WHERE EmpId = @EmpId AND [Year] = YEAR(@WorkDate)))
            BEGIN
                UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
                SET [Minutes] = [Minutes] - @OldEarlyOut + @EarlyOut, ModifiedDate = dbo.fnc_GetClientDateTime()
                WHERE EmpId = @EmpId AND [Year] >= YEAR(@WorkDate)
            END
            ELSE
            BEGIN
                SELECT @MinutesPreviousYear = [Minutes]
                FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
                WHERE EmpId = @EmpId 
                    AND [Year] = YEAR(@WorkDate) - 1

                INSERT INTO dbo.LateInEarlyOutYearly (EmpId, [Year], [Minutes], ModifiedDate)
                VALUES (@EmpId, YEAR(@WorkDate), (ISNULL(@MinutesPreviousYear, 0) - @OldEarlyOut + @EarlyOut), dbo.fnc_GetClientDateTime())
            END

            DELETE FROM dbo.LateInEarlyOutTrans 
			WHERE EmpId = @EmpId 
			AND WorkDate = @WorkDate 
			AND [Type] = 2
			AND ISNULL(IsManual, 0) = 0

            IF(@EarlyOut <> 0)
            BEGIN
                INSERT INTO dbo.LateInEarlyOutTrans (EmpId, WorkDate, [Type], [Minutes], ModifiedDate)
                VALUES (@EmpId, @WorkDate, 2, @EarlyOut, dbo.fnc_GetClientDateTime())
            END
        END
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LateInEarlyOut_Update] @Id INT
    ,@WorkDate DATETIME
    ,@Type TINYINT
    ,@Minutes TINYINT
    ,@EmpId INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    UPDATE dbo.LateInEarlyOut
    SET dbo.LateInEarlyOut.WorkDate = @WorkDate
        ,-- date
        dbo.LateInEarlyOut.Type = @Type
        ,-- tinyint
        dbo.LateInEarlyOut.Minutes = @Minutes
        ,-- tinyint
        dbo.LateInEarlyOut.EmpId = @EmpId
        ,-- int
        dbo.LateInEarlyOut.ModifiedBy = @ModifiedBy
        ,-- int
        dbo.LateInEarlyOut.ModifiedDate = dbo.fnc_GetClientDateTime()
    WHERE dbo.LateInEarlyOut.Id = @Id;

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update Late in/Early out failed.';
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = '';
    END;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Leave_ClearExpiredTransfer]
    @Year INT
AS
/*
Created: 20250411@HenryDinh
Task: Clear unsued transfer annual leave  
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @TransferDesc NVARCHAR(100) = 'Transfer AL from ' + CAST(@Year AS VARCHAR)

    CREATE TABLE #LeaveToClear (
        EmpId INT,
		NickName NVARCHAR(100),
        TotalTransferred NUMERIC(5,1),
        TotalUsed NUMERIC(5,1),
        Remaining NUMERIC(5,1),
        TransferId INT
    )

	;WITH CombinedLeaveTrans AS (
        SELECT lt.Id, EmpId, CreditDays, DeductDays, FromDate, ToDate, Description, Deleted
        FROM NexHRM.dbo.LeaveTrans AS lt WITH (NOLOCK)
		INNER JOIN NexHRM.dbo.Employee AS emp WITH (NOLOCK) ON emp.Id = lt.EmpId
        WHERE ISNULL(Deleted, 0) = 0 AND emp.IsSpecialUser = 0
        UNION ALL
        SELECT lt.Id, EmpId, CreditDays, DeductDays, FromDate, ToDate, Description, Deleted
        FROM LumiHRM.dbo.LeaveTrans AS lt WITH (NOLOCK) 
        INNER JOIN LumiHRM.dbo.Employee AS emp WITH (NOLOCK) ON emp.Id = lt.EmpId
        WHERE ISNULL(Deleted, 0) = 0 AND emp.IsSpecialUser = 0
    )
    INSERT INTO #LeaveToClear (EmpId, NickName, TotalTransferred, TotalUsed, Remaining, TransferId)
    SELECT
        lt.EmpId,
		e.NickName, 
        SUM(ISNULL(lt.CreditDays, 0)) AS TotalTransferred,
        (
            SELECT SUM(ISNULL(DeductDays, 0))
            FROM CombinedLeaveTrans AS used
            WHERE used.EmpId = lt.EmpId
                AND ISNULL(used.Deleted, 0) = 0
                AND used.DeductDays > 0
                AND used.FromDate >= lt.FromDate
                AND used.ToDate <= lt.ToDate
        ) AS TotalUsed,
        0 AS Remaining,
        MAX(lt.Id)
    FROM CombinedLeaveTrans lt
    INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.Id = lt.EmpId 
    WHERE lt.Description = @TransferDesc
		AND ISNULL(e.IsDeleted, 0) = 0
		AND e.Enabled = 1
        AND e.IsSpecialUser = 0
    GROUP BY lt.EmpId, e.NickName, lt.Description, lt.FromDate, lt.ToDate

    UPDATE t
    SET Remaining = ISNULL(TotalTransferred, 0) - ISNULL(TotalUsed, 0)
    FROM #LeaveToClear t

    INSERT INTO dbo.LeaveTrans (
        EmpId,
        RequestId,
        Description,
        FromDate,
        ToDate,
        DeductDays,
        CreditDays,
        Deleted,
        CreatedDate,
        CreatedBy,
		ModifiedDate,
		ModifiedBy,
        IsOpenBalance,
        IsCloseBalance
    )
    SELECT
        EmpId,
        NULL AS RequestId,
        'Auto clear unused transfer AL from ' + CAST(@Year AS VARCHAR),
        '2025-04-01 00:00:00',
        '2025-04-01 00:00:00',
        Remaining,
        0,
        NULL,
        '2025-04-01 00:00:00',
        NULL,
		'2025-04-01 00:00:00',
        NULL,
        0,
        0
    FROM #LeaveToClear
    WHERE Remaining > 0

    DROP TABLE #LeaveToClear
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LeaveConversion_Get] 
	@FromDate DATETIME
	,@ToDate DATETIME
	,@Type TINYINT
	,@GroupId INT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20140810@Quinn.Vo
Task: Get Leave deduction.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	
	SELECT 
		ld.Id AS LeaveConversionId,
		ld.[Date],
		ld.IsAnnual,
		ld.LeaveDays,
		ld.[Description],
		ld.ModifiedDate,
		ld.EmpId,
		e.NickName,
		e.FullName
		,g.Code AS TeamCode
		,g.Name AS TeamName
	FROM dbo.LeaveConversion AS ld WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ld.EmpId = e.Id
	INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
	WHERE ld.[Type] = @Type
	AND DATEDIFF(D, @FromDate, ld.[Date]) >= 0
	AND DATEDIFF(D, ld.[Date], @ToDate) >= 0
	AND (
			@EmployeeId IS NULL
			OR e.Id = @EmployeeId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LeaveDeduction_Delete] 
	@LeaveDeductionId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140604@Quinn.Vo
Task: Update Leave deduction.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @OldTotalDays NUMERIC(10, 1), @OldIsAnnual BIT, @EmployeeId INT, @DeductDate DATETIME

	SELECT @OldTotalDays = LeaveDays, @OldIsAnnual = IsAnnual, @EmployeeId = EmpId, @DeductDate = [Date]
	FROM dbo.LeaveConversion WITH (NOLOCK)
	WHERE Id = @LeaveDeductionId 

	SET XACT_ABORT ON
	BEGIN TRAN
	BEGIN TRY
		DELETE FROM dbo.LeaveConversion WHERE Id = @LeaveDeductionId
		DELETE FROM dbo.LateInEarlyOutTrans WHERE LeaveDeductionId = @LeaveDeductionId
		DELETE FROM dbo.LeaveTrans WHERE LeaveConversionId = @LeaveDeductionId

		UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
		SET [Minutes] = [Minutes] + (@OldTotalDays * 8 * 60), ModifiedDate = dbo.fnc_GetClientDateTime(), ModifiedBy = @ModifiedBy
		WHERE EmpId = @EmployeeId AND [Year] >= YEAR(@DeductDate)

		COMMIT
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK

			SELECT @ErrCode = 100
				,@ErrMsg = 'There is an error when deleting leave deduction.'
		END
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LeaveDeduction_Get] 
	@FromDate DATETIME
	,@ToDate DATETIME
	,@GroupId INT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20140810@Quinn.Vo
Task: Get Leave deduction.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	
	SELECT 
		ld.Id AS LeaveDeductionId,
		ld.[Date] AS DeductDate,
		ld.IsAnnual,
		ld.LeaveDays,
		ld.[Description],
		--ld.ModifiedBy,
		ld.ModifiedDate,
		ld.EmpId,
		e.NickName,
		e.FullName
		,e.GroupId
		,g.Code
		,g.Name
		,ld.ModifiedBy AS ModifiedById
		,modi.NickName
		,modi.FullName
	FROM dbo.LeaveConversion AS ld WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ld.EmpId = e.Id
	INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
	LEFT JOIN dbo.Employee AS modi WITH (NOLOCK) ON ld.ModifiedBy = modi.Id
	WHERE [Type] = 1
	AND DATEDIFF(D, @FromDate, ld.[Date]) >= 0
	AND DATEDIFF(D, ld.[Date], @ToDate) >= 0
	AND (
			@EmployeeId IS NULL
			OR e.Id = @EmployeeId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LeaveDeduction_Insert] @EmployeeId INT
    ,@Description NVARCHAR(4000)
    ,@DeductDate DATETIME
    ,@TotalDays NUMERIC(18, 1)
    ,@IsAnnual BIT
    ,@CreatedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140604@Quinn.Vo
Task: Insert Leave deduction.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id INT
        ,@TotalDayRequestPending NUMERIC(10, 1)
        ,@MinutesPreviousYear INT
		,@AnnualLeaveBalance NUMERIC(10, 1)

	SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE EmpId = @EmployeeId 
		AND ISNULL(Deleted, 0) = 0  
		AND YEAR(FromDate) = YEAR(@DeductDate)

    SELECT @TotalDayRequestPending = SUM(TotalDays)
    FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
        LEFT JOIN dbo.LeaveType AS ty WITH (NOLOCK) ON rq.LeaveTypeID = ty.Id
    WHERE rq.EmpId = @EmployeeId
        AND [Status] = 1
        AND RequestType = 6
        AND ty.IsALDeduction = 1
	
    IF (
            @IsAnnual = 1
            AND (@AnnualLeaveBalance - ISNULL(@TotalDayRequestPending, 0)) < @TotalDays
            )
    BEGIN
        SELECT @ErrCode = 100
            ,@ErrMsg = 'Selected employee does not have enough accumulated annual leave.'

        RETURN
    END

    SET XACT_ABORT ON

    BEGIN TRAN

    BEGIN TRY
        INSERT INTO dbo.LeaveConversion (
            EmpId
            ,[Date]
            ,IsAnnual
            ,LeaveDays
            ,IsDeleted
            ,ModifiedBy
            ,ModifiedDate
            ,[Description]
            ,[Type]
            )
        SELECT @EmployeeId
            ,@DeductDate
            ,@IsAnnual
            ,@TotalDays
            ,0
            ,@CreatedBy
            ,dbo.fnc_GetClientDateTime()
            ,@Description
            ,1

        SELECT @Id = CAST(SCOPE_IDENTITY() AS INT)

        INSERT INTO dbo.LateInEarlyOutTrans (
            EmpId
            ,WorkDate
            ,[Type]
            ,[Minutes]
            ,LeaveDeductionId
            ,ModifiedBy
            ,ModifiedDate
            )
        VALUES (
            @EmployeeId
            ,@DeductDate
            ,0
            ,(@TotalDays * 8 * 60)
            ,@Id
            ,@CreatedBy
            ,dbo.fnc_GetClientDateTime()
            )

        IF(EXISTS (SELECT 1 FROM dbo.LateInEarlyOutYearly WITH (NOLOCK) WHERE EmpId = @EmployeeId AND [Year] = YEAR(@DeductDate)))
        BEGIN
            UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
            SET [Minutes] = [Minutes] - (@TotalDays * 8 * 60), ModifiedDate = dbo.fnc_GetClientDateTime()
            WHERE EmpId = @EmployeeId AND [Year] >= YEAR(@DeductDate)
        END
        ELSE
        BEGIN
            SELECT @MinutesPreviousYear = [Minutes]
            FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
            WHERE EmpId = @EmployeeId 
                AND [Year] = YEAR(@DeductDate) - 1

            INSERT INTO dbo.LateInEarlyOutYearly (EmpId, [Year], [Minutes], ModifiedDate)
            VALUES (@EmployeeId, YEAR(@DeductDate), (ISNULL(@MinutesPreviousYear, 0) - (@TotalDays * 8 * 60)), dbo.fnc_GetClientDateTime())
        END

        IF (@IsAnnual = 1)
        BEGIN
            INSERT INTO dbo.LeaveTrans (
                EmpId
                ,Description
                ,FromDate
                ,DeductDays
                ,CreditDays
                ,CreatedDate
                ,CreatedBy
                ,RequestId
                ,LeaveConversionId
                )
            SELECT @EmployeeId
                ,@Description
                ,@DeductDate
                ,@TotalDays
                ,0
                ,dbo.fnc_GetClientDateTime()
                ,@CreatedBy
                ,- 1
                ,@Id
        END

        COMMIT
    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK

            SELECT @ErrCode = 100
                ,@ErrMsg = 'There is an error when inserting leave deduction.'
        END
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LeaveDeduction_Update] 
    @LeaveDeductionId INT
    ,@Description NVARCHAR(4000)
    ,@DeductDate DATETIME
    ,@TotalDays NUMERIC(18, 1)
    ,@IsAnnual BIT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140604@Quinn.Vo
Task: Update Leave deduction.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;
    DECLARE @Id INT, @AnnualLeaveBalance NUMERIC(10, 1),
		@TotalDayRequestPending NUMERIC(10, 1), @OldTotalDays NUMERIC(10, 1), @OldIsAnnual BIT, @EmployeeId INT,
    @OldDeductDate DATETIME, @MinutesPreviousYear INT, @oldMinutes INT

    SELECT @OldTotalDays = LeaveDays, @oldMinutes = (LeaveDays * 8 * 60) , @OldIsAnnual = IsAnnual, @EmployeeId = EmpId, @OldDeductDate = [Date]
    FROM dbo.LeaveConversion WITH (NOLOCK)
    WHERE Id = @LeaveDeductionId 

    SELECT @TotalDayRequestPending = SUM(TotalDays)
    FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
    LEFT JOIN dbo.LeaveType AS ty WITH (NOLOCK) ON rq.LeaveTypeID = ty.Id
    WHERE rq.EmpId = @EmployeeId
        AND [Status] = 1
        AND RequestType = 6
        AND ty.IsALDeduction = 1
	
    IF (@OldIsAnnual = 0)
    BEGIN
        SET @OldTotalDays = 0
    END

	SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE EmpId = @EmployeeId 
		AND ISNULL(Deleted, 0) = 0  
		AND YEAR(FromDate) = YEAR(@DeductDate)

	SELECT @TotalDayRequestPending = SUM(ISNULL(wk.TotalDays, 0))
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON e.Id = wk.EmpId
	WHERE e.Id = @EmployeeId
		AND wk.RequestType = 6
		AND wk.[Status] = 1
		AND wk.LeaveTypeID IN (2, 30)
	GROUP BY e.Id

    SELECT @OldTotalDays = ISNULL(@OldTotalDays, 0), @TotalDayRequestPending =  ISNULL(@TotalDayRequestPending, 0)

    IF (@IsAnnual = 1 
        AND (@AnnualLeaveBalance + @OldTotalDays - @TotalDayRequestPending) < @TotalDays)
    BEGIN
        SELECT @ErrCode = 100
                ,@ErrMsg = 'Selected employee does not have enough accumulated annual leave.'
        RETURN
    END

    SET XACT_ABORT ON
    BEGIN TRAN
    BEGIN TRY
        UPDATE dbo.LeaveConversion WITH (ROWLOCK, UPDLOCK)
        SET EmpId = @EmployeeId,
            [Date] = @DeductDate,
            IsAnnual = @IsAnnual,
            LeaveDays = @TotalDays,
            [Description] = @Description,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE Id = @LeaveDeductionId

        UPDATE dbo.LateInEarlyOutTrans WITH (ROWLOCK, UPDLOCK)
        SET EmpId = @EmployeeId,
            WorkDate = @DeductDate,
            [Minutes] = (@TotalDays * 8 * 60),
            LeaveDeductionId = @LeaveDeductionId,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE LeaveDeductionId = @LeaveDeductionId

        UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
            SET [Minutes] = [Minutes] + @oldMinutes, ModifiedDate = dbo.fnc_GetClientDateTime()
            WHERE EmpId = @EmployeeId AND [Year] >= YEAR(@OldDeductDate)

        IF(EXISTS (SELECT 1 FROM dbo.LateInEarlyOutYearly WITH (NOLOCK) WHERE EmpId = @EmployeeId AND [Year] = YEAR(@DeductDate)))
        BEGIN
            UPDATE dbo.LateInEarlyOutYearly WITH (ROWLOCK, UPDLOCK)
            SET [Minutes] = [Minutes] - (@TotalDays * 8 * 60), ModifiedDate = dbo.fnc_GetClientDateTime()
            WHERE EmpId = @EmployeeId AND [Year] >= YEAR(@DeductDate)
        END
        ELSE
        BEGIN
            SELECT @MinutesPreviousYear = [Minutes]
            FROM dbo.LateInEarlyOutYearly WITH (NOLOCK)
            WHERE EmpId = @EmployeeId 
                AND [Year] = YEAR(@DeductDate) - 1

            INSERT INTO dbo.LateInEarlyOutYearly (EmpId, [Year], [Minutes], ModifiedDate)
            VALUES (@EmployeeId, YEAR(@DeductDate), (ISNULL(@MinutesPreviousYear, 0) - (@TotalDays * 8 * 60)), dbo.fnc_GetClientDateTime())
        END

        IF(@IsAnnual = 1 OR @OldIsAnnual = 1)
        BEGIN
        DELETE FROM dbo.LeaveTrans WHERE LeaveConversionId = @LeaveDeductionId
        IF(@IsAnnual = 1)
        BEGIN
            INSERT INTO dbo.LeaveTrans (
                EmpId
                ,Description
                ,FromDate
                ,DeductDays
                ,CreditDays
                ,CreatedDate
                ,CreatedBy
                ,RequestId
                ,LeaveConversionId
                )
            SELECT @EmployeeId
                ,@Description
                ,@DeductDate
                ,@TotalDays
                ,0
                ,dbo.fnc_GetClientDateTime()
                ,@ModifiedBy
                ,-1
                ,@LeaveDeductionId
        END
        END

        COMMIT
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK

            SELECT @ErrCode = 100
                ,@ErrMsg = 'There is an error when updating leave deduction.'
        END
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[LeaveType_GetAll]
    @ExcludeNotShown bit = null
AS
BEGIN
    SET NOCOUNT ON;

    select * from LeaveType
    where (@ExcludeNotShown is null or isShown=1)
    order by [name] 
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Log_WriteLog]
    @LogActionID int,
    @LogDate datetime,
    @Description NVARCHAR(4000),
    @TrackingValue NVARCHAR(MAX),
    @UserID int
AS
/*
Created: 2012/04/20@Dan
Task: write log
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

INSERT INTO [dbo].[Log]
           ([LogActionID]
           ,[LogDate]
           ,[Description]
           ,[TrackingValue]
           ,[UserID])
     VALUES
           (
            @LogActionID
           ,@LogDate
           ,@Description
           ,@TrackingValue
           ,@UserID
            )
GO

CREATE OR ALTER PROCEDURE [dbo].[LogReachedWorkingTime_Insert] @EmployeeIds VARCHAR(MAX)
	,@WorkDate DATETIME
	,@InsertedType VARCHAR(20)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190516@Quinn.Vo
Task: insert log for remind reached working time
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.LogReachedWorkingTime (
		EmpId
		,WorkDate
		,InsertedDate
		,InsertedType
		)
	SELECT val
		,@WorkDate
		,dbo.fnc_GetClientDateTime()
		,@InsertedType
	FROM dbo.fnc_strSplit(@EmployeeIds, ',')
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Memo_Delete] @Id      INT,
                                    @ErrCode INT          = 0 OUTPUT,
                                    @ErrMsg  VARCHAR(200) = '' OUTPUT
AS
     BEGIN
         DELETE MEMO
         WHERE Id = @Id;
         
         IF @@ERROR <> 0
             BEGIN                 
                 SELECT @ErrCode = 1,
                        @ErrMsg = N'Delete Memo failed.';
             END;
         ELSE
             BEGIN                 
                 SELECT @ErrCode = 0,
                        @ErrMsg = '';
             END;         
     END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Memo_GetAll] @content NVARCHAR = NULL
    ,@year INT
    ,@category INT
    ,@status INT = - 1
AS
BEGIN
    SELECT m.[Id]
      ,[RefNo]
      ,[Contents]
      ,[Subject]
      ,[SentTo]
      ,[SentFrom]
      ,[ForInfo]
      ,[TotalPages]
      ,[SentDate]
      ,[Category]
      ,[Type] AS MemoType
      ,[Enabled]
      ,[Status]
        ,lk.[Name] AS CategoryName
    FROM MEMO m WITH (NOLOCK)
        INNER JOIN HRM.dbo.LookupData lk WITH (NOLOCK) ON lk.id = m.Category
    WHERE  (
            @year = - 1
            OR year(m.SentDate) = @year
            )
        AND (
            @category = - 1
            OR m.Category = @category
            )
        AND Enabled = 1
        AND (
            @status = - 1
            OR m.STATUS = @status
            )
    ORDER BY m.SentDate DESC
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Memo_GetById]
    @Id INT
AS
BEGIN
    SELECT m.[Id]
      ,[RefNo]
      ,[Contents]
      ,[Subject]
      ,[SentTo]
      ,[SentFrom]
      ,[ForInfo]
      ,[TotalPages]
      ,[SentDate]
      ,[Category]
      ,[Type] AS MemoType
      ,[Enabled]
      ,[Status]
	  , lk.[Name] AS CategoryName 
    FROM dbo.MEMO m WITH(nolock) 
    INNER JOIN HRM.dbo.LookupData lk WITH(nolock) ON lk.id = m.Category 
    WHERE m.Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Memo_Insert] @RefNo      NVARCHAR(50),
                                    @Contents   NVARCHAR(MAX),
                                    @Subject    NVARCHAR(4000),
                                    @SentTo     NVARCHAR(1000),
                                    @SentFrom   NVARCHAR(1000),
                                    @ForInfo    NVARCHAR(1000),
                                    @TotalPages INT,
                                    @SentDate   DATETIME,
                                    @Category   INT,
                                    @Type       INT,
                                    @Status     BIT,
                                    @MemoId     INT            = 0 OUTPUT,
                                    @ErrCode    INT            = 0 OUTPUT,
                                    @ErrMsg     VARCHAR(200)   = '' OUTPUT
AS
     BEGIN
         INSERT INTO MEMO
         (RefNo,
          Contents,
          Subject,
          SentTo,
          SentFrom,
          ForInfo,
          TotalPages,
          SentDate,
          Category,
          [Type],
          [Status]
         )
         VALUES
         (@RefNo,
          @Contents,
          @Subject,
          @SentTo,
          @SentFrom,
          @ForInfo,
          @TotalPages,
          @SentDate,
          @Category,
          @Type,
          @Status
         );
         DECLARE @Id INT;
         IF @@ERROR <> 0
             BEGIN
                 SET @Id = 0;
                 SELECT @ErrCode = 1,
                        @ErrMsg = N'Create Memo failed.';
             END;
         ELSE
             BEGIN
                 SET @Id = CAST(SCOPE_IDENTITY() AS INT);
                 SELECT @ErrCode = 0,
                        @ErrMsg = '';
             END;
         SET @MemoId = @Id;
     END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Memo_Update] @Id INT
    ,@RefNo NVARCHAR(50)
    ,@Contents NVARCHAR(MAX)
    ,@Subject NVARCHAR(4000)
    ,@SentTo NVARCHAR(1000)
    ,@SentFrom NVARCHAR(1000)
    ,@ForInfo NVARCHAR(1000)
    ,@TotalPages INT
    ,@SentDate DATETIME
    ,@Category INT
    ,@Type INT
    ,@Status BIT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
BEGIN
    UPDATE MEMO
    SET RefNo = @RefNo
        ,Contents = @Contents
        ,Subject = @Subject
        ,SentTo = @SentTo
        ,SentFrom = @SentFrom
        ,ForInfo = @ForInfo
        ,TotalPages = @TotalPages
        ,SentDate = @SentDate
        ,Category = @Category
        ,[Type] = @Type
        ,[Status] = @status
    WHERE Id = @Id;

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update Memo failed.';
    END;
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = '';
    END;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[MessageCategoryLog_CheckIfObjectNotChange]
	@categoryId INT
	,@jsonObject NVARCHAR(MAX)
AS
/*
Created: 20200708@Merlin.Le
Task: Check if the object have not change
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 1 
	FROM [dbo].[MessageCategoryLog] AS l WITH (NOLOCK) 
	WHERE l.MessageCategory = @categoryId AND l.JsonObject = @jsonObject;		
END
GO

CREATE OR ALTER PROCEDURE [dbo].[MessageCategoryLog_GetJsonObjectByCategoryId]
	@categoryId INT
AS
/*
Created: 20200708Merlin.Le
Task: Get Json object by category id
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	SELECT l.JsonObject
	FROM [dbo].[MessageCategoryLog] AS l WITH (NOLOCK)
	WHERE l.MessageCategory = @categoryId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[MessageCategoryLog_UpdateJsonObject]
	@categoryId INT
	,@jsonObject NVARCHAR(MAX)
	,@errCode INT = 0
	,@errMsg VARCHAR (50) = ''
AS
/*
Created: 20200708Merlin.Le
Task: Update Json Object
DB: NexHRM
*/
BEGIN
	SET NOCOUNT ON;

	IF (EXISTS 
		(SELECT 1 
		FROM [dbo].[MessageCategoryLog] AS l WITH (NOLOCK) 
		WHERE l.MessageCategory = @categoryId)
		)
		BEGIN
			UPDATE [dbo].[MessageCategoryLog]
				SET JsonObject = @jsonObject
				WHERE MessageCategory = @categoryId
		END
	ELSE
		BEGIN
			INSERT INTO [dbo].[MessageCategoryLog] (MessageCategory, JsonObject)
			VALUES(@categoryId, @jsonObject)
		END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Notification_GetAllViolation] @EmpId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('#LateinEarlyOutViolationInYear') IS NOT NULL
	BEGIN
		DROP TABLE #LateinEarlyOutViolationInYear;
	END;

	CREATE TABLE #LateinEarlyOutViolationInYear (
		EmpId INT
		,NickName VARCHAR(100)
		,Fullname NVARCHAR(max)
		,TimesWithinAYear INT
		);

	IF OBJECT_ID('#LateinEarlyOutViolationInMonth') IS NOT NULL
	BEGIN
		DROP TABLE #LateinEarlyOutViolationInMonth;
	END;

	CREATE TABLE #LateinEarlyOutViolationInMonth (
		EmpId INT
		,TimesWithinThisMonth INT
		);

	IF OBJECT_ID('#CheckInOutViolation') IS NOT NULL
	BEGIN
		DROP TABLE #CheckInOutViolation;
	END;

	CREATE TABLE #CheckInOutViolation (
		EmpId INT
		,TimesMissingThisMonth INT
		);

	IF OBJECT_ID('#ToeicExpired') IS NOT NULL
	BEGIN
		DROP TABLE #ToeicExpired;
	END;

	CREATE TABLE #ToeicExpired (
		EmpId INT
		,IsExpired BIT
		);
--------
IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary;
	END;

	CREATE TABLE #LateInEarlyOutSummary (
		EmpId INT
		,[Month] INT
		,[Year] INT
		,TotalTimes INT
		);

	INSERT INTO #LateInEarlyOutSummary
	SELECT DISTINCT tr.EmpId
		,MONTH(tr.WorkDate)
		,YEAR(tr.WorkDate)
		,COUNT(*)
	FROM dbo.LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
		AND e.GroupId NOT IN (1081, 40) -- management, expat
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
	WHERE tr.[Type] IN (1, 2)
		AND CAST(tr.WorkDate AS DATE) BETWEEN DATEADD(m, - 12, dbo.fnc_GetClientDateTime())
			AND dbo.fnc_GetClientDateTime()
		AND e.Id = @EmpId
	GROUP BY tr.EmpId
		,MONTH(tr.WorkDate)
		,YEAR(tr.WorkDate)
	HAVING COUNT(*) >= 4
---------
	--late in/early out violation--
	INSERT INTO #LateinEarlyOutViolationInYear (
		EmpId
		,NickName
		,Fullname
		,TimesWithinAYear
		)
	SELECT e.Id
		,e.NickName
		,e.FullName
		,COUNT(tr.TotalTimes) AS TotalTimes
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN #LateInEarlyOutSummary AS tr ON e.Id = tr.EmpId
	WHERE 
	   --e.GroupId NOT IN (11, 40) -- management, expat
		ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND e.Id = @EmpId
	GROUP BY e.Id
		,e.NickName
		,e.FullName
		,e.Code
		,e.Roles
		,e.GroupId
		,e.GroupDate
		,g.Code
		,g.[Name]
		,g.[Rank]
	ORDER BY g.[Rank]
		,g.Code
		,e.Roles DESC
		,e.GroupDate;

	---------------
	DECLARE @StartOfMonth DATETIME
		,@EndOfMonth DATETIME

	SET @StartOfMonth = DATEADD(month, DATEDIFF(month, 0, dbo.fnc_GetClientDateTime()), 0)
	SET @EndOfMonth = DATEADD(month, DATEDIFF(month, 0, dbo.fnc_GetClientDateTime()) + 1, - 1)

	INSERT INTO #LateinEarlyOutViolationInMonth (
		EmpId
		,TimesWithinThisMonth
		)
	SELECT e.Id
		,COUNT(lieot.WorkDate) AS 'TimesWithinThisMonth'
	FROM dbo.Employee e
		LEFT JOIN (
		SELECT *
		FROM dbo.LateInEarlyOutTrans lieot
		WHERE lieot.Type IN (1, 2)
			AND lieot.WorkDate BETWEEN @StartOfMonth
				AND @EndOfMonth
		) lieot ON e.Id = lieot.EmpId
	WHERE e.Enabled = 1
		AND e.Id = @EmpId
	GROUP BY e.id
		,e.NickName
		,e.FullName

	-----------------------
	DECLARE @missingCheckInOut TABLE (
		WorkDate DATETIME
		,isCurrent BIT
		,TypeId BIT
		,LeaveDay FLOAT
		,EmpId INT
		,Nickname VARCHAR(100)
		,Fullname NVARCHAR(max)
		)

	INSERT @missingCheckInOut
	EXEC [dbo].[Report_EmployeeSpaceDashBoard] @EmpId = @EmpId

	INSERT INTO #CheckInOutViolation (
		EmpId
		,TimesMissingThisMonth
		)
	SELECT e.Id
		,COUNT(mcio.WorkDate) 'TimesMissingInThisMonth'
	FROM dbo.Employee e
		LEFT JOIN (
		SELECT *
		FROM @missingCheckInOut mcio
		WHERE mcio.isCurrent = 1
			AND mcio.TypeId = 3
		) mcio ON e.Id = mcio.EmpId
	WHERE e.Enabled = 1
		AND e.Id = @EmpId
	GROUP BY e.Id
		,e.NickName
		,e.FullName



	SELECT leoviy.TimesWithinAYear AS 'LateInEarlyOutInYear'
		,leovim.TimesWithinThisMonth AS 'LateInEarlyOutInMonth'
		,ciov.TimesMissingThisMonth AS 'TimeMisssingCheckInOutThisMonth'
		,leoviy.EmpId
		,leoviy.NickName
		,leoviy.Fullname		
	FROM #LateinEarlyOutViolationInYear leoviy
		JOIN #LateinEarlyOutViolationInMonth leovim ON leoviy.EmpId = leovim.EmpId
		JOIN #CheckInOutViolation ciov ON leoviy.EmpId = ciov.EmpId
		JOIN #ToeicExpired te ON leoviy.EmpId = te.EmpId

	DROP TABLE #LateinEarlyOutViolationInYear;

	DROP TABLE #LateinEarlyOutViolationInMonth;

	DROP TABLE #CheckInOutViolation;

	DROP TABLE #ToeicExpired;
	
	DROP TABLE #LateInEarlyOutSummary;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_ApproveRequest] 
	@Id             INT
    ,@ModifiedBy    INT
    ,@ErrCode       INT				= 0 OUTPUT
    ,@ErrMsg        VARCHAR(200)	= '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
	- 20200316Merlin.Le: Enhance NEX balance request
	- 20200731Dwight.Tran: Enhance NEX Request  
	- 20250327@Henry: Update MGT Code -> MGT/COR code
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE
		@PendingStatus		INT = 1
		,@ApprovedStatus	INT	= 2;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
				AND [Status] = @PendingStatus
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can approve the pending NEX request';

		RETURN;
	END;

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
			WHERE e.[Id] = @ModifiedBy
				AND (eg.[Code] = 'COR' OR eg.[Code] = 'MGT')
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only managers have a permisson to aprove NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE [dbo].[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET
            [Status]        = @ApprovedStatus
            ,[ModifiedBy]   = @ModifiedBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE [OvertimeId] = @Id AND [Status]=1 -- pending;

		COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Approve NEX request failed';

		ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_Delete]
    @Id                 INT
    ,@ModifiedId        INT
    ,@ErrCode           INT                 = 0 OUTPUT
    ,@ErrMsg            VARCHAR(200)        = '' OUTPUT
AS
/*
Created: 20200302Merlin.Le
Task: Enhance NEX balance request
DB: NexHRM
Original:
Revisions: 20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
	DECLARE
        @PendingStatus		INT = 1;

    IF (
        NOT EXISTS (
            SELECT 1 FROM [dbo].[Overtime] WITH (NOLOCK)
            WHERE [OvertimeId] = @Id
        )
    )
    BEGIN
        SELECT
            @ErrCode = 1
            ,@ErrMsg = 'NEX request not exist';

        RETURN;
    END;

	IF (
        NOT EXISTS (
            SELECT 1 FROM [dbo].[Overtime] WITH (NOLOCK)
            WHERE [OvertimeId] = @Id
                AND [CreatedBy] = @ModifiedId
        )
    )
    BEGIN
        SELECT
            @ErrCode = 2
            ,@ErrMsg = 'You only have a permission to delete your NEX request';

        RETURN;
    END;

	IF (
		NOT EXISTS (
            SELECT 1 FROM [dbo].[Overtime] WITH (NOLOCK)
            WHERE [OvertimeId] = @Id
                AND [Status] = @PendingStatus
		)
	)
	BEGIN
        SELECT
            @ErrCode = 3
            ,@ErrMsg = 'You only can delete pending NEX request';

        RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        DELETE FROM [dbo].[Overtime]
        WHERE [OvertimeId] = @Id;

        DELETE FROM [dbo].[OvertimeDetail]
        WHERE [OvertimeId] = @Id;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Delete NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_DeleteConversion]
    @modifiedBy     INT
    ,@id            INT
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
Created: 20200723@Dwight.Tran
Task: Enhance NEX request
DB: NexHRM
Original: 

Revisions:    
	- 20250327@Henry: Update MGT Code -> MGT/COR code
    - 20250507@Henry.Dinh: update HR code to HR-ADM
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @empId     INT
        ,@year          INT
        ,@oldMinutes    INT
		,@HRCode    VARCHAR (2) = 'HR-ADM'

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @modifiedBy
            AND (
                eg.[Code] = @HRCode
                OR eg.[Code] = 'COR'
				OR eg.[Code] = 'MGT'
                OR e.[Roles] LIKE '%3%'
                OR e.[Roles] LIKE '%4%'
            )
    )
    BEGIN
        SELECT
            @errCode = 1
            ,@errMsg = 'Only role HR have a permission to delete OT conversion';

        RETURN;
    END;

    IF NOT EXISTS(
        SELECT 1 FROM [dbo].[OvertimeConversion] WITH (NOLOCK)
        WHERE [Id] = @id
    )
    BEGIN
        SELECT
            @errCode = 2
            ,@errMsg = 'OT conversion not exist';

        RETURN;
    END;

    SELECT
        @empId     = [EmpId]
        ,@year          = Year([DeductDate])
        ,@oldMinutes    = [Deduct]
    FROM [dbo].[OvertimeConversion] WITH (NOLOCK)
    WHERE [Id] = @id;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK)
        WHERE [EmpId] = @empId
            AND [Year] = @year
    )
    BEGIN
        SELECT
            @errCode = 3
            ,@errMsg = 'Overtime balance in ' + CAST(@year AS VARCHAR (4)) + ' not exist';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM [dbo].[OvertimeConversion]
        WHERE [Id] = @id;

		UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
        SET
            [Minutes]       = [Minutes] + @oldMinutes
            ,[ModifiedBy]   = @modifiedBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE [EmpId] = @empId
            AND [Year] >= @year;
        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @errCode = 1000
            ,@errMsg = 'Delete OT conversion fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_DeleteTransaction]
	@OvertimeId INT
	,@ModifiedBy INT
AS
/*
Created: 20160309@Quinn.Vo
Task: Delete OT transaction.
DB: NexHRM
Original: 

Revisions:  20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(
		SELECT 1
				FROM [dbo].[OvertimeTrans] WITH (NOLOCK)
				WHERE [OvertimeId] = @OvertimeId
	)
	BEGIN
		IF OBJECT_ID('##OvertimeDetail') IS NOT NULL
		BEGIN
			DROP TABLE #OvertimeDetail
		END

		CREATE TABLE #OvertimeDetail (
			EmpId INT
			,[Date] DATETIME
			,[Minutes] INT
			)

		INSERT INTO #OvertimeDetail (
			EmpId
			,[Date]
			,[Minutes]
			)
		SELECT otd.EmployeeId
			,otd.[Date]
			,otd.[NexHour] * 60
		FROM dbo.[Overtime] AS ot WITH (NOLOCK)
			INNER JOIN dbo.[OvertimeDetail] AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
		WHERE ot.OvertimeId = @OvertimeId

		DELETE
		FROM dbo.OvertimeTrans
		WHERE OvertimeId = @OvertimeId

		UPDATE oty
		SET oty.[Minutes] = (oty.[Minutes] - otd.[Minutes])
			,oty.[ModifiedBy] = @ModifiedBy
			,oty.[ModifiedDate] = dbo.fnc_GetClientDateTime()
		FROM #OvertimeDetail AS otd
			INNER JOIN dbo.OvertimeYearly AS oty WITH (UPDLOCK, ROWLOCK) ON oty.EmpId = otd.EmpId
			AND [Year] >= YEAR(otd.[Date])
		WHERE EXISTS (
				SELECT 1
				FROM dbo.OvertimeYearly WITH (NOLOCK)
				WHERE EmpId = otd.EmpId
					AND [Year] = YEAR(otd.[Date])
				)

		DROP TABLE #OvertimeDetail
	END;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_DeleteTransactionFromLeaveRequest] @RequestId INT
	,@EmployeeId INT
	,@Date DATE
	,@TotalDays NUMERIC(18, 1)
	,@ModifiedBy INT
AS
/*
Created: 20160314@Quinn.Vo
Task: Delete OT transaction from Leave request.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DELETE
	FROM dbo.OvertimeTrans
	WHERE LeaveConversionId = @RequestId

	UPDATE dbo.OvertimeYearly
	SET [Minutes] = ([Minutes] + (@TotalDays * 480))
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = dbo.fnc_GetClientDateTime()
	WHERE [Year] >= YEAR(@Date)
		AND EmpId = @EmployeeId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_DisapproveRequest] 
	@Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
	- 20200316Merlin.Le: Enhance NEX balance request
	- 20200903Dwight.Tran: Enhance NEX Request.
	- 20250507@Henry.Dinh: update group id from 11 to 1081
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SET NOCOUNT ON;

    DECLARE
		@GroupManagerId		INT	= 1081
        ,@PendingStatus     INT = 1
        ,@ApprovedStatus	INT = 2;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
				AND [Status] = @ApprovedStatus
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can disapprove approved NEX request';

		RETURN;
	END;

	IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			WHERE e.[Id] = @ModifiedBy
				AND (e.[GroupId] = @GroupManagerId OR 
					(e.Roles IN (3, 4))) -- HR, SA
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only managers or HR have a permisson to disapproved NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET [Status] = @PendingStatus
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE OvertimeId = @Id;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT @ErrCode = 1000
            ,@ErrMsg = 'Disapprove NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_GetById] @Id INT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original:

Revisions: 20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT ot.OvertimeId AS Id
        ,ot.[Status]
        ,ot.[Description]
		,ot.[CreatedDate]
        ,ot.CreatedBy AS CreatedById
        ,e.NickName AS CreatedByNickName
        ,e.FullName AS CreatedByFullName
    FROM dbo.[Overtime] ot WITH (NOLOCK)
        LEFT JOIN Employee e WITH (NOLOCK) ON e.Id = ot.CreatedBy
    WHERE ot.OvertimeId = @Id;

    SELECT otd.OvertimeId AS Id
        ,otd.[Hour]
		,otd.[Date]
		,otd.[NexHour]
		,otd.Note
		,otd.EmployeeId RecordEmployeeId
        ,otd.EmployeeId 
        ,e.NickName
        ,e.FullName
    FROM dbo.[OvertimeDetail] otd WITH (NOLOCK)
        LEFT JOIN Employee e WITH (NOLOCK) ON e.Id = otd.EmployeeId
    WHERE otd.OvertimeId = @Id;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_GetConversions]
	@fromDate		DATE
	,@toDate		DATE
    ,@employeeId    INT				= NULL
	,@GroupId		INT				= NULL
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
Created: 
Task: 
DB: NexHRM
Original: 

Revisions:  20200723@Dwight.Tran - Enhance NEX request
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
		otc.[Id]
		,otc.[Deduct] / 60.0	AS Deduct
		,otc.[DeductDate]
		,otc.[Description]
		,otc.[ModifiedDate]
		,otc.[EmpId] AS EmployeeId
		,e.NickName
		,e.FullName
		,e.GroupId
		,g.Code AS TeamCode
		,g.Name	AS TeamName
	FROM [dbo].[OvertimeConversion] AS otc WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON otc.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN dbo.Employee AS modi WITH (NOLOCK) ON otc.ModifiedBy = modi.Id
	WHERE DATEDIFF(DAY, @fromDate, otc.[DeductDate]) >= 0
		AND DATEDIFF(DAY, otc.[DeductDate], @toDate) >= 0
		AND (
			@employeeId IS NULL
			OR otc.[EmpId] = @employeeId
		)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_GetNexHour]
    @employeeId INT
	,@hour	NUMERIC(18,1)
	,@date	DATETIME
/*
Created: 20200306@Merlin.Le
Task: Enhance Nex Leave
DB: NexHRM
Original: 

Revisions:  20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE 
		@realHourOT			NUMERIC(18,1) = 0
		,@minHour			NUMERIC(18,1);

	IF EXISTS(
		SELECT 1 FROM dbo.[EmployeeCalendar]
		WHERE DATEDIFF(DAY, @date, WorkDate) = 0
		AND ( IsOffDay = 1 OR IsHoliday = 1) 
		AND EmpId = @employeeId
	)
	BEGIN
		SELECT 0 as [Value], 0 as [IsWorkDay]
	END

	ELSE
	BEGIN
		SELECT @hour*1.5 as [Value], 1 as [IsWorkDay]
	END
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_GetSummary] 
	@year			INT
	,@employeeId	INT = NULL
	,@groupId		INT = NULL
	,@departmentId	INT = NULL
AS
/*
	Created: 
	Task: 
	DB: NexHRM
	Original:
	Revisions:
		- 20200723Dwight.Tran: Enhance NEX request
		- 20250507@Henry.Dinh: update group id from 11 to 1081
	Param's Explanation:
*/
BEGIN
	

	IF OBJECT_ID('#PreviousYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #PreviousYearBalance
	END

	CREATE TABLE #PreviousYearBalance (
		EmpId INT
		,[Minutes] INT
		,ConversionValue INT
		)

	IF OBJECT_ID('#CurrentYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #CurrentYearBalance
	END

	CREATE TABLE #CurrentYearBalance (
		EmpId INT
		,[Minutes] INT
		,LateInEarlyOut INT
		)

	INSERT INTO #PreviousYearBalance (
		EmpId
		,[Minutes]
		,ConversionValue
		)
	SELECT e.Id
		,(
			ISNULL(oty.[Minutes], 0) - CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN ISNULL(la.[Minutes], 0)
			ELSE ISNULL(oty.[Minutes], 0) END
			)
		,CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN ISNULL(la.[Minutes], 0)
			ELSE ISNULL(oty.[Minutes], 0)
			END
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = (@Year - 1)
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = (@Year - 1)
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@groupId IS NULL
			OR e.GroupId = @groupId
			)
		AND (
			@departmentId IS NULL
			OR eg.DepartmentId = @departmentId
			)

	INSERT INTO #CurrentYearBalance (
		EmpId
		,[Minutes]
		,LateInEarlyOut
		)
	SELECT e.Id
		,CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN (ISNULL(oty.[Minutes], 0) - ISNULL(la.[Minutes], 0))
			ELSE 0
			END
		,(CASE 
			WHEN ISNULL(oty.[Minutes], 0) > ISNULL(la.[Minutes], 0) THEN ISNULL(la.[Minutes], 0)
			ELSE ISNULL(oty.[Minutes], 0) END
			)
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = @Year
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = @year
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@groupId IS NULL
			OR e.GroupId = @groupId
			)
		AND (
			@departmentId IS NULL
			OR eg.DepartmentId = @departmentId
			)

	SELECT ott.EmpId AS EmployeeId
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 1
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Jan'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 2
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Feb'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 3
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Mar'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 4
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Apr'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 5
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'May'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 6
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Jun'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 7
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Jul'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 8
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Aug'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 9
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Sep'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 10
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Oct'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 11
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Nov'
		,SUM(CASE 
				WHEN MONTH(ott.[Date]) = 12
					AND ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'Dec'
		,SUM(CASE 
				WHEN ott.[Type] = 1 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'TotalHour'
		,SUM(CASE 
				WHEN ott.[Type] = 0 THEN ISNULL([ott].[Minutes], 0)
				ELSE 0
				END) 'LeaveConversion'
	INTO #OvertimeSummary
	FROM [dbo].[OvertimeTrans] AS ott
	WHERE YEAR(ott.[Date]) = @year
	GROUP BY ott.EmpId

	IF OBJECT_ID('#TempOvertimeConversion') IS NOT NULL
	BEGIN
		DROP TABLE #TempOvertimeConversion;
	END;

	CREATE TABLE #TempOvertimeConversion (
		[EmpId]	INT
		,[Deduct]		INT
	);

	INSERT INTO #TempOvertimeConversion
	SELECT
		[EmpId]
		,SUM([Deduct])
	FROM [dbo].[OvertimeConversion]
	WHERE Year([DeductDate]) = @year
	GROUP BY [EmpId]

	SELECT ISNULL(os.Jan, 0) / 60.0 AS 'Jan'
		,ISNULL(os.Feb, 0) / 60.0 AS 'Feb'
		,ISNULL(os.Mar, 0) / 60.0 AS 'Mar'
		,ISNULL(os.Apr, 0) / 60.0 AS 'Apr'
		,ISNULL(os.May, 0) / 60.0 AS 'May'
		,ISNULL(os.Jun, 0) / 60.0 AS 'Jun'
		,ISNULL(os.Jul, 0) / 60.0 AS 'Jul'
		,ISNULL(os.Aug, 0) / 60.0 AS 'Aug'
		,ISNULL(os.Sep, 0) / 60.0 AS 'Sep'
		,ISNULL(os.Oct, 0) / 60.0 AS 'Oct'
		,ISNULL(os.Nov, 0) / 60.0 AS 'Nov'
		,ISNULL(os.DEC, 0) / 60.0 AS 'Dec'
		,ISNULL(os.TotalHour, 0) / 60.0 AS TotalHour
		,ISNULL(os.LeaveConversion, 0) / 60.0 AS LeaveConversion
		,ISNULL(prev.Minutes, 0) / 60.0 AS BalanceLastYear
		,ISNULL(cur.[Minutes], 0) / 60.0 AS BalanceCurrentYear
		,ISNULL(cur.LateInEarlyOut, 0) / 60.0 AS LateInEarlyOut
		,ISNULL(totc.[Deduct], 0) / 60.0 AS OvertimeDeduct
		,e.Id AS EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
		,eg.DepartmentId
		,d.Name AS DepartmentName
		,e.GroupId AS TeamId
		,eg.Code AS TeamCode
		,eg.NAME AS TeamName
	FROM dbo.Employee e
		INNER JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
		LEFT JOIN dbo.Department AS d WITH (NOLOCK) ON eg.DepartmentId = d.DepartmentId
		LEFT JOIN #OvertimeSummary AS os WITH (NOLOCK) ON e.Id = os.EmployeeId
		LEFT JOIN #CurrentYearBalance AS cur WITH (NOLOCK) ON e.Id = cur.EmpId
		LEFT JOIN #PreviousYearBalance AS prev WITH (NOLOCK) ON e.Id = prev.EmpId
		LEFT JOIN #TempOvertimeConversion AS totc WITH (NOLOCK) ON e.[Id] = totc.[EmpId]
	WHERE e.IsSpecialUser = 0
		AND  (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@groupid IS NULL
			OR e.GroupId = @groupId
			)
		AND (
			@departmentId IS NULL
			OR eg.DepartmentId = @departmentId
			)
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.GroupId NOT IN (1081, 40)
	ORDER BY eg.[Rank]
		,eg.Code
		,e.Roles DESC
		,e.GroupDate

	DROP TABLE #OvertimeSummary

	DROP TABLE #PreviousYearBalance

	DROP TABLE #CurrentYearBalance

	DROP TABLE #TempOvertimeConversion
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_Insert]
    @CreatedBy			INT
    ,@Description       NVARCHAR (4000)
    ,@OvertimeSheet		NVARCHAR (MAX)
    ,@ErrCode			INT                 = 0 OUTPUT
    ,@ErrMsg			VARCHAR(200)        = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
    - 20200227@Merlin.Le: Enhance NEX leave
	- 20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @OTRequestId    INT
        ,@EmployeeId    INT
        ,@Date          DATE
        ,@FromTime      VARCHAR (8)
        ,@ToTime        VARCHAR (8)
        ,@Hour          NUMERIC (18, 1)
		,@NexdHour		NUMERIC (18, 1)
        ,@Note          NVARCHAR (250)
        ,@DocHandler    INT;

	IF OBJECT_ID('#OvertimeSheet') IS NOT NULL
	BEGIN
		DROP TABLE #OvertimeSheet;
	END;

    CREATE TABLE #OvertimeSheet (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[Hour]         NUMERIC (18, 1)
        ,[NexHour]      NUMERIC (18, 1)
        ,[Note]         NVARCHAR (250)
    );

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
                LEFT JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON eg.[Id] = e.[GroupId]
            WHERE e.[Id] = @CreatedBy
                AND (
                    e.[Roles] like '%1%'		-- Assistant leader
                    OR e.[Roles] like '%2%'		-- Leader
                    OR e.[Roles] like '%4%'		-- SA
                    OR eg.[Code] = 'PM'
                )
        )
    )
    BEGIN
        SELECT
            @ErrCode = 1
            ,@ErrMsg = 'PM team, Assistant Leader, Leader must have a permission to create the NEX request';

        RETURN;
    END;

    EXEC sp_xml_preparedocument @DocHandler OUTPUT, @OvertimeSheet;
     
    INSERT INTO #OvertimeSheet
    SELECT
        [EmployeeId]
        ,[Date]
        ,[Hour]
        ,[NexHour]
        ,[Note]
    FROM OPENXML(@DocHandler, '/Root/OvertimeRecord', 1)
    WITH (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[Hour]         NUMERIC (18, 1)
        ,[NexHour]      NUMERIC (18, 1)
        ,[Note]         NVARCHAR (250)
    );

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Overtime] (
            [Description]
            ,[Status]
            ,[CreatedBy]
            ,[CreatedDate]
        )
        VALUES (
            @Description
            ,1
            ,@CreatedBy
            ,dbo.fnc_GetClientDateTime()
        );

        SELECT @OTRequestId = SCOPE_IDENTITY();

        WHILE (
            EXISTS (
                SELECT 1
                FROM #OvertimeSheet
            )
        )
        BEGIN
            SELECT TOP (1)
                @EmployeeId = [EmployeeId]
                ,@Date      = [Date]
                ,@Hour      = [Hour]
				,@NexdHour	= [NexHour]
                ,@Note      = [Note]
            FROM #OvertimeSheet;

            IF (
                EXISTS (
                    SELECT 1
                    FROM [dbo].[OvertimeDetail] AS otd WITH (NOLOCK)
                        LEFT JOIN [dbo].[Overtime] AS ot WITH (NOLOCK) ON ot.[OvertimeId] = otd.[OvertimeId]
                    WHERE otd.[EmployeeId] = @EmployeeId
						AND DATEDIFF(DAY, otd.[Date], @Date) = 0
                        AND ot.Status IN (1, 2, 4)
                )
            )
            BEGIN
                SELECT
                    @ErrCode = 2
                    ,@ErrMsg = 'Employee ' + NickName + ' has already worked over time on ' + CAST(@Date AS VARCHAR (10))
                FROM Employee WITH (NOLOCK)
                WHERE Id = @EmployeeId;

                ROLLBACK TRANSACTION;
                RETURN;
            END;

            INSERT INTO [dbo].[OverTimeDetail] (
                [OvertimeId]
                ,[EmployeeId]
                ,[Date]
                ,[Hour]
				,[NexHour]
                ,[Note]
            )
            VALUES (
                @OTRequestId
                ,@EmployeeId
                ,@Date
                ,@Hour
				,@NexdHour
                ,@Note
            );

            DELETE TOP (1)
            FROM #OvertimeSheet
            WHERE
				EmployeeId = @EmployeeId
				AND DATEDIFF(DAY, [Date], @Date) = 0
        END;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Insert NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_InsertConversion]
    @createdBy      INT
    ,@empId    INT
    ,@deduct        NUMERIC (18, 1)
	,@deductDate DATETIME
    ,@description   VARCHAR (250)
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
    Created: 20200723@Dwight.Tran
    Task: Enhance NEX request
    DB: NexHRM
    Original: 

    Revisions:    
        - 20250327@Henry: Update MGT Code -> MGT/COR code
        - 20250507@Henry.Dinh: update HR code to HR-ADM
    Param's Explanation:
*/
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @oldMinutes INT
        ,@HRCode    VARCHAR (2) = 'HR-ADM'
		,@year INT = YEAR(@deductDate);

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @createdBy
            AND (
                eg.[Code] = @HRCode
                OR eg.[Code] = 'COR'
				OR eg.[Code] = 'MGT'
                OR e.[Roles] LIKE '%3%'
                OR e.[Roles] LIKE '%4%'
            )
    )
    BEGIN
        SELECT
            @errCode = 1
            ,@errMsg = 'Only role HR have a permission to deduct NEX balance';

        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK)
        WHERE [EmpId] = @empId
            AND [Year] = @year
    )
    BEGIN
        SELECT
            @errCode = 2
            ,@errMsg = 'Overtime balance in ' + CAST(@year AS VARCHAR (4)) + ' not exist';

        RETURN;
    END;

	
    SELECT @oldMinutes = [Minutes]
    FROM [dbo].[OvertimeYearly]
    WHERE [EmpId] = @empId
        AND [Year] = @year;

    IF (@oldMinutes < (@deduct * 60.0))
    BEGIN
        SELECT
            @errCode = 3
            ,@errMsg = 'Not enough NEX balance to deduct';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[OvertimeConversion] (
            [EmpId]
            ,[Deduct]
            ,[DeductDate]
            ,[Description]
			,[ModifiedBy]
			,[ModifiedDate]
        )
        VALUES (
            @empId
            ,@deduct * 60.0
            ,@deductDate
            ,@description
			,@createdBy,
			dbo.fnc_GetClientDateTime()
        )

		UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
        SET
            [Minutes]       = [Minutes] - (@deduct * 60.0)
            ,[ModifiedBy]   = @createdBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE [EmpId] = @empId
            AND [Year] >= @year;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @errCode = 1000
            ,@errMsg = 'Deduct NEX balance fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_InsertTransactionFromLeaveRequest] @RequestId INT
	,@EmployeeId INT
	,@Date DATE
	,@TotalDays NUMERIC(18, 1)
	,@ModifiedBy INT
AS
/*
Created: 20160314@Quinn.Vo
Task: Insert OT transaction from Leave request.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	INSERT INTO dbo.OvertimeTrans (
		EmpId
		,[Date]
		,[Minutes]
		,LeaveConversionId
		,[Type]
		,ModifiedBy
		,ModifiedDate
		)
	SELECT @EmployeeId
		,@Date
		,@TotalDays * 480
		,@RequestId
		,0
		,@ModifiedBy
		,dbo.fnc_GetClientDateTime()

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.OvertimeYearly WITH (NOLOCK)
				WHERE [Year] = YEAR(@Date)
					AND EmpId = @EmployeeId
				)
			)
	BEGIN
		UPDATE dbo.OvertimeYearly
		SET [Minutes] = ([Minutes] - (@TotalDays * 480))
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = dbo.fnc_GetClientDateTime()
		WHERE [Year] >= YEAR(@Date)
		AND EmpId = @EmployeeId
	END
	ELSE
	BEGIN
		INSERT INTO dbo.OvertimeYearly (
			EmpId
			,[Year]
			,[Minutes]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		SELECT EmpId
			,YEAR(@Date)
			,[Minutes] - (@TotalDays * 480)
			,@ModifiedBy
			,dbo.fnc_GetClientDateTime()
		FROM dbo.OvertimeYearly WITH (NOLOCK)
		WHERE EmpId = @EmployeeId
			AND ([Year] = YEAR(@Date) - 1)
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_MyReport]
    @FromDate       DATETIME
    ,@ToDate        DATETIME
    ,@EmployeeId    INT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original:

Revisions: 20200731Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT
        ot.OvertimeId   AS Id
        ,otd.[Date]      AS OnDate
        ,ot.[Status]
        ,ot.[Description]
        ,otd.[Hour]
        ,otd.EmployeeId
		,e.NickName 
        ,e.FullName
        ,e1.NickName AS CreatedBy

    FROM dbo.[Overtime] ot WITH (NOLOCK)
        INNER JOIN dbo.Employee e1 WITH (NOLOCK) ON e1.Id = ot.CreatedBy
        INNER JOIN dbo.[OvertimeDetail] otd WITH (NOLOCK) ON otd.OvertimeId = ot.OvertimeId
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON otd.EmployeeId = e.Id
    WHERE ot.[Status] IN (1, 2)
		AND DATEDIFF(DAY, @FromDate, otd.[Date]) >= 0
		AND DATEDIFF(DAY, @ToDate, otd.[Date]) <= 0
        AND otd.EmployeeId = @EmployeeId;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_RejectRequest] @Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
	- 20200316Merlin.Le: Enhance NEX balance request
	- 20200731Dwight.Tran: Enhance NEX Request.  
	- 20250327@Henry: Update MGT Code -> MGT/COR code
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @RejectStatus      INT = 3
        ,@PendingStatus		INT = 1;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE OvertimeId = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE OvertimeId = @Id
				AND [Status] = @PendingStatus
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can reject pending NEX request';

		RETURN;
	END;

    IF (
        NOT EXISTS (
			SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
			WHERE e.[Id] = @ModifiedBy
				AND (eg.[Code] = 'COR' OR eg.[Code] = 'MGT')
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only managers have a permisson to reject NEX request'

		RETURN;
	END;

    BEGIN TRY
        UPDATE [dbo].[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET
            [Status]        = @RejectStatus
            ,[ModifiedBy]   = @ModifiedBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE OvertimeId = @Id;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Reject OT request failed';
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_ReportDetail]
    @FromDate       DATETIME
    ,@ToDate        DATETIME
    ,@Status        INT
    ,@CreatedBy     INT = NULL
    ,@EmployeeId    INT = NULL
	,@OvertimeId	INT = NULL
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original:

Revisions:
    - 20200227@Merlin.Le: Enhance NEX leave
	- 20200903Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT
        ot.OvertimeId 
        ,otd.[Date]          AS OnDate
        ,ot.[Status]
        ,ot.[Description]
        ,otd.[Hour]
		,otd.[Note]
        ,otd.EmployeeId
        ,e2.NickName
        ,e2.FullName
        ,e1.NickName AS CreatedBy
    FROM dbo.[Overtime] ot WITH (NOLOCK)
        LEFT JOIN dbo.OvertimeDetail otd WITH (NOLOCK) ON otd.OvertimeId = ot.OvertimeId
        LEFT JOIN Employee e1 WITH (NOLOCK) ON e1.Id = ot.CreatedBy
        LEFT JOIN Employee e2 WITH (NOLOCK) ON e2.Id = otd.EmployeeId
    WHERE
		(
            @Status = 0
            OR ot.[Status] = @Status
        )
        AND (
            @CreatedBy IS NULL
            OR ot.CreatedBy = @CreatedBy
        )
        AND (
            @EmployeeId IS NULL
            OR otd.EmployeeId = @EmployeeId
        )
		AND (
			(@OvertimeId IS NULL AND DATEDIFF(DAY, @FromDate, otd.[Date]) >= 0
			AND DATEDIFF(DAY, @ToDate, otd.[Date]) <=0)
			OR otd.OvertimeId = @OvertimeId
		);
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_ReportHR] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@CreatedBy INT = NULL
    ,@Status INT = 0
AS
/*
Created: 20200903Dwight.Tran
Task: Enhance NEX Request.
DB: NexHRM
Original:

Revisions: 
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	IF OBJECT_ID('#TempOvertime') IS NOT NULL
	BEGIN
		DROP TABLE #TempOvertime
	END

	CREATE TABLE #TempOvertime (
		Id INT
		,[Status] NVARCHAR (250)
		,[Description] NVARCHAR (250)
		,[CreatedDate] DATE
		,[CreatedId] INT
		,[NickName] NVARCHAR (250)
		,[FullName] NVARCHAR (250)
		)

	INSERT INTO #TempOvertime (
		Id 
		,[Status] 
		,[Description] 
		,[CreatedDate] 
		,[CreatedId] 
		,[NickName] 
		,[FullName]
		)
    SELECT DISTINCT  ot.OvertimeId AS Id
        ,ot.[Status]
        ,ot.[Description]
		,ot.[CreatedDate]
        ,ot.CreatedBy AS CreatedById
        ,e.NickName AS CreatedByNickName
        ,e.FullName AS CreatedByFullName
    FROM dbo.[Overtime] AS ot WITH (NOLOCK)
		INNER JOIN dbo.[OvertimeDetail] AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
        LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ot.CreatedBy
    WHERE DATEDIFF(DAY, @FromDate, otd.[Date]) >= 0
		AND DATEDIFF(DAY, @ToDate, otd.[Date]) <= 0
        AND (
            (@Status = 0
            OR ot.[Status] = @Status)
			AND ot.[Status] IN (2, 4, 5)	--approved, verified, void
            )
        AND (
            @CreatedBy IS NULL
            OR ot.CreatedBy = @CreatedBy
            );

	SELECT Id 
		,[Status] 
		,[Description] 
		,[CreatedDate] 
		,[CreatedId] AS CreatedById
		,[NickName] AS CreatedByNickName
		,[FullName] AS CreatedByFullName
	FROM #TempOvertime

    SELECT otd.OvertimeId
        ,otd.[Hour]
		,otd.[Date]
        ,otd.EmployeeId
        ,e.NickName
        ,e.FullName
    FROM dbo.[Overtime] AS ot WITH (NOLOCK)
		INNER JOIN #TempOvertime as temp WITH (NOLOCK) ON ot.[OvertimeId] = temp.[Id]
        INNER JOIN dbo.[OvertimeDetail] AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
        LEFT JOIN Employee AS e WITH (NOLOCK) ON e.Id = otd.EmployeeId

	DROP TABLE #TempOvertime
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_ReportHRDetail]
    @Id		INT
/*
Created: 20200903Dwight.Tran
Task: Enhance NEX Request
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

	IF OBJECT_ID('##TempHourOT') IS NOT NULL
	BEGIN
		DROP TABLE #TempHourOT
	END

	CREATE TABLE #TempHourOT (
		EmployeeId INT
		,RealHourOT INT
		,[Date]		DATE
		)

	INSERT INTO #TempHourOT (
		EmployeeId 
		,RealHourOT 
		,[Date]
		)
	SELECT
		otd.[EmployeeId]
		,(CASE
			WHEN ISNULL(ec.[IsHoliday], 0) = 1 OR ISNULL(ec.[IsOffDay], 0) = 1
				THEN 0
			WHEN ea.[TotalWorkHours] IS NULL AND ISNULL(ec.[LeaveDay], 0) <> 1
				THEN otd.[Hour]
			WHEN ISNULL(ec.[LeaveDay], 0) = 1
				THEN IIF(otd.[Hour] < DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60)
							, otd.[Hour], DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60))
			WHEN ISNULL(ec.[LeaveDay], 0) = 0.5
				THEN IIF(otd.[Hour] < (DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60) + ea.[LateIn] + ea.[EarlyOut] - 4)
							,otd.[Hour], (DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60) + ea.[LateIn] + ea.[EarlyOut] - 4))
			WHEN ISNULL(ec.[LeaveDay], 0) = 0
				THEN IIF(otd.[Hour] < (DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60) - 1 + ea.[LateIn] + ea.[EarlyOut] - 8)
							,otd.[Hour], (DATEDIFF(MINUTE, ea.[InFixed], ea.[OutFixed])/CONVERT(DECIMAL(4,2),60) - 1 + ea.[LateIn] + ea.[EarlyOut] - 8))
			ELSE otd.[Hour]
			END) as RealHourOT
		,otd.[Date]
	FROM dbo.[OvertimeDetail] as otd WITH (NOLOCK)
		LEFT JOIN dbo.[EmployeeAttendance] as ea WITH (NOLOCK)
		ON ea.[EmpId] = otd.[EmployeeId] AND DATEDIFF(DAY, ea.[AttendanceDate], otd.[Date]) = 0
		LEFT JOIN dbo.[EmployeeCalendar] as ec WITH (NOLOCK)
		ON ec.[EmpId] = otd.[EmployeeId] AND DATEDIFF(DAY, ec.[WorkDate], otd.[Date]) = 0
	WHERE otd.[OvertimeId] = @Id
	
	SELECT
		otd.[OvertimeId]
		,otd.[Note]
		,otd.[Date]
		,otd.[Hour]
		,(CASE
			WHEN ot.[Status] = 4	-- verified
				THEN otd.[NexHour]
			ELSE (FLOOR(2 * temp.[RealHourOT]/2)*1.5) END) as 'NexHour'
		,otd.[EmployeeId] as UserId	-- employee
		,emp.[NickName]
		,emp.[FullName]
	FROM dbo.[Overtime] as ot WITh (NOLOCK)
		INNER JOIN dbo.[OvertimeDetail] as otd WITH (NOLOCK)
		ON ot.[OvertimeId] = otd.[OvertimeId]
		INNER JOIN #TempHourOT as temp WITH (NOLOCK)
		ON temp.[EmployeeId] = otd.[EmployeeId] AND temp.[Date] = otd.[Date]
		INNER JOIN dbo.[Employee] as emp WITH (NOLOCK)
		ON emp.[Id] = otd.[EmployeeId]
	WHERE otd.[OvertimeId] = @Id

	DROP TABLE #TempHourOT;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_ReportSummary] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@CreatedBy INT = NULL
    ,@Status INT = 0
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original:

Revisions: 20200903Dwight.Tran: Enhance NEX Request.
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT  ot.OvertimeId AS Id
        ,ot.[Status]
        ,ot.[Description]
		,ot.[CreatedDate]
        ,ot.CreatedBy AS CreatedById
        ,e.NickName AS CreatedByNickName
        ,e.FullName AS CreatedByFullName
    FROM dbo.[Overtime] AS ot WITH (NOLOCK)
		INNER JOIN dbo.[OvertimeDetail] AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
        LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ot.CreatedBy
    WHERE DATEDIFF(DAY, @FromDate, otd.[Date]) >= 0
		AND DATEDIFF(DAY, @ToDate, otd.[Date]) <= 0
        AND (
            @Status = 0
            OR ot.[Status] = @Status
            )
        AND (
            @CreatedBy IS NULL
            OR ot.CreatedBy = @CreatedBy
            );

    SELECT otd.OvertimeId
        ,otd.[Hour]
		,otd.[Date]
        ,otd.EmployeeId
        ,e.NickName
        ,e.FullName
    FROM dbo.[Overtime] AS ot WITH (NOLOCK)
        INNER JOIN dbo.[OvertimeDetail] AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
        LEFT JOIN Employee AS e WITH (NOLOCK) ON e.Id = otd.EmployeeId
    WHERE DATEDIFF(DAY, @FromDate, otd.[Date]) >= 0
		AND DATEDIFF(DAY, @ToDate, otd.[Date]) <= 0
        AND (
            @Status = 0
            OR ot.[Status] = @Status
            )
        AND (
            @CreatedBy IS NULL
            OR ot.CreatedBy = @CreatedBy
            );
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_ResetRequest] 
	@Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
	Created: 20200903Dwight.Tran
	Task: Enhance NEX Request.
	DB: NexHRM
	Original: 

	Revisions:  
		- 20250507@Henry.Dinh: update group id from 11 to 1081
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SET NOCOUNT ON;

    DECLARE
		@GroupManagerId		INT	= 1081
        ,@PendingStatus		INT = 1;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
				AND [Status] BETWEEN 2 AND 5
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can reset NEX request when status is not pending';

		RETURN;
	END;

	IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			WHERE e.[Id] = @ModifiedBy
				AND (e.[GroupId] = @GroupManagerId OR 
					(e.Roles IN (3, 4))) -- HR, SA
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only role HR have a permission to void NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET [Status] = @PendingStatus
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE OvertimeId = @Id;

		EXECUTE dbo.[Overtime_DeleteTransaction] @Id, @ModifiedBy;
        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT @ErrCode = 1000
            ,@ErrMsg = 'Reset NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_Update]
    @Id                 INT
    ,@ModifiedBy        INT
    ,@Description       NVARCHAR(4000)
    ,@OverTimeSheet     VARCHAR(MAX)
    ,@ErrCode           INT                 = 0 OUTPUT
    ,@ErrMsg            VARCHAR(200)        = '' OUTPUT
AS
/*
Created: 20160129@Razor.Nguyen
Task: OT Feature.
DB: NexHRM
Original: 

Revisions:  
	- 20200302Merlin.Le: Enhance NEX balance request
	- 20200731Dwight.Tran: Enhance NEX Request.  
	- 20250327@Henry: Update MGT Code -> MGT/COR code
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE
		@EmployeeId     INT
        ,@Date          DATE
        ,@FromTime      VARCHAR (8)
        ,@ToTime        VARCHAR (8)
        ,@Hour          NUMERIC (18, 1)
		,@NexHour		NUMERIC	(18, 1)
        ,@Note          NVARCHAR (250)
        ,@DocHandler    INT
        ,@PendingStatus INT             = 1;

	IF OBJECT_ID('#OvertimeSheet') IS NOT NULL
	BEGIN
		DROP TABLE #OvertimeSheet;
	END;

    CREATE TABLE #OvertimeSheet (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[Hour]         NUMERIC (18, 1)
		,[NexHour]		NUMERIC	(18, 1)
        ,[Note]         NVARCHAR (250)
    );

	IF (
		NOT EXISTS (
			SELECT 1 FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
        SELECT
            @ErrCode = 1
            ,@ErrMsg = 'NEX request not exist';

        RETURN;
	END;

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Overtime] ot WITH (NOLOCK)
            WHERE
				ot.[CreatedBy] = @ModifiedBy
				AND ot.[OvertimeId] = @Id
        )
    )
    BEGIN
        SELECT
            @ErrCode = 2
            ,@ErrMsg = 'You only have a permission to update your NEX request';

        RETURN;
    END;

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Overtime] ot WITH (NOLOCK)
            WHERE ot.[OvertimeId] = @Id
                AND ot.[Status] = @PendingStatus
        )
    )
    BEGIN
        SELECT
            @ErrCode = 3
            ,@ErrMsg = 'You only can update pending NEX request';

        RETURN;
    END;

    EXEC sp_xml_preparedocument @DocHandler OUTPUT, @OvertimeSheet;
     
    INSERT INTO #OvertimeSheet
    SELECT
        [EmployeeId]
        ,[Date]
        ,[Hour]
		,[NexHour]
        ,[Note]
    FROM OPENXML(@DocHandler, '/Root/OvertimeRecord', 1)
    WITH (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[Hour]         NUMERIC (18, 1)
		,[NexHour]		NUMERIC (18, 1)
        ,[Note]         NVARCHAR (250)
    );

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE [dbo].[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET
			[Description] = @Description
            ,[ModifiedBy] = @ModifiedBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
		WHERE [OvertimeId] = @Id

		DELETE FROM [dbo].[OvertimeDetail]
			WHERE [OvertimeId] = @Id

        WHILE (
            EXISTS (
                SELECT 1
                FROM #OvertimeSheet
            )
        )
        BEGIN
            SELECT TOP (1)
                @EmployeeId = [EmployeeId]
                ,@Date      = [Date]
                ,@Hour      = [Hour]
				,@NexHour	= [NexHour]
                ,@Note      = [Note]
            FROM #OvertimeSheet;

            IF (
                EXISTS (
                    SELECT 1
                    FROM [dbo].[OverTimeDetail] AS otd WITH (NOLOCK)
                        LEFT JOIN [dbo].[Overtime] AS ot WITH (NOLOCK) ON ot.[OvertimeId] = otd.[OvertimeId]
                    WHERE otd.[EmployeeId] = @EmployeeId
						AND DATEDIFF(DAY, @Date, otd.[Date]) = 0
                        AND ot.Status IN (1, 2, 4)
                )
            )
            BEGIN
                SELECT
                    @ErrCode = 4
                    ,@ErrMsg = 'Employee ' + NickName + ' has already worked over time on ' + CONVERT(VARCHAR, @Date, 106)
                FROM Employee WITH (NOLOCK)
                WHERE Id = @EmployeeId;

                ROLLBACK TRANSACTION;
                RETURN;
            END;

            INSERT INTO [dbo].[OvertimeDetail] (
                [OvertimeId]
                ,[EmployeeId]
                ,[Date]
                ,[Hour]
				,[NexHour]
                ,[Note]
            )
            VALUES (
                @Id
                ,@EmployeeId
                ,@Date
                ,@Hour
				,@NexHour
                ,@Note
            );

            DELETE TOP (1)
            FROM #OvertimeSheet
            WHERE
				EmployeeId = @EmployeeId
				AND DATEDIFF(DAY, @Date, [Date]) = 0
        END;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Update NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_UpdateConversion]
    @modifiedBy     INT
    ,@id            INT
    ,@deduct        NUMERIC (18, 1)
	,@deductDate	DATETIME
    ,@description   VARCHAR (250)
    ,@errCode       INT             = 0 OUTPUT
    ,@errMsg        VARCHAR (250)   = '' OUTPUT
/*
    Created: 20200723@Dwight.Tran
    Task: Enhance NEX request
    DB: NexHRM
    Original: 

    Revisions:    
        - 20250327@Henry: Update MGT Code -> MGT/COR code
        - 20250507@Henry.Dinh: update HR code to HR-ADM
    Param's Explanation:

*/
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @oldYearlyMinutes   INT
        ,@empId				INT
        ,@year              INT = Year(@deductDate)
		,@oldYear			INT
        ,@oldMinutes        INT
		,@HRCode    VARCHAR (2) = 'HR-ADM';

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[Employee] e WITH (NOLOCK)
            INNER JOIN [dbo].[EmployeeGroup] eg WITH (NOLOCK) ON e.[GroupId] = eg.[Id]
        WHERE e.[Id] = @modifiedBy
            AND (
                eg.[Code] = @HRCode
                OR eg.[Code] = 'COR'
                OR eg.[Code] = 'MGT'
                OR e.[Roles] LIKE '%3%'
                OR e.[Roles] LIKE '%4%'
            )
    )
    BEGIN
        SELECT
            @errCode = 1
            ,@errMsg = 'Only role HR have a permission to update OT conversion';

        RETURN;
    END;

    IF NOT EXISTS(
        SELECT 1 FROM [dbo].[OvertimeConversion] WITH (NOLOCK)
        WHERE [Id] = @id
    )
    BEGIN
        SELECT
            @errCode = 2
            ,@errMsg = 'OT conversion not exist';

        RETURN;
    END;

    SELECT
        @empId = [EmpId]
		,@oldYear = Year([DeductDate])
		,@oldMinutes = [Deduct]
    FROM [dbo].[OvertimeConversion] WITH (NOLOCK)
    WHERE [Id] = @id;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[OvertimeYearly] WITH (NOLOCK)
        WHERE [EmpId] = @empId
            AND [Year] = @year
    )
    BEGIN
        SELECT
            @errCode = 3
            ,@errMsg = 'Overtime balance in ' + CAST(@year AS VARCHAR (4)) + ' not exist';

        RETURN;
    END;

    SELECT @oldYearlyMinutes = [Minutes]
    FROM [dbo].[OvertimeYearly] as oty WITH (NOLOCK)
    WHERE oty.[EmpId] = @empId
        AND oty.[Year] = @year;

    IF (@oldYearlyMinutes + @oldMinutes < @deduct * 60.0)
    BEGIN
        SELECT
            @errCode = 4
            ,@errMsg = 'Not enough NEX balance to deduct';

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;
		
		UPDATE [dbo].[OvertimeConversion] WITH (ROWLOCK, UPDLOCK)
		SET
			[Deduct]        = @deduct * 60
			,[DeductDate] = @deductDate
			,[Description]  = @description
			,[ModifiedDate] = dbo.fnc_GetClientDateTime()
			,[ModifiedBy] = @modifiedBy
		WHERE [Id] = @id;

		UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
        SET
            [Minutes]       = [Minutes] + @oldMinutes - (@deduct * 60.0)
            ,[ModifiedBy]   = @modifiedBy
            ,[ModifiedDate] = dbo.fnc_GetClientDateTime()
        WHERE [EmpId] = @empId
            AND [Year] >= @year;

		IF(@oldYear < @year)
		BEGIN
			UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
			SET
				[Minutes]       = [Minutes] + @oldMinutes
				,[ModifiedBy]   = @modifiedBy
				,[ModifiedDate] = dbo.fnc_GetClientDateTime()
			WHERE [EmpId] = @empId
				AND [Year] >= @oldYear AND [Year] < @year;
		END;
		COMMIT TRANSACTION;
        
    END TRY

    BEGIN CATCH
        SELECT
            @errCode = 1000
            ,@errMsg = 'Update OT conversion fail';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_VerifyRequest]
    @Id					INT
	,@ModifiedBy		INT
    ,@OvertimeSheet		NVARCHAR (MAX)
    ,@ErrCode			INT                 = 0 OUTPUT
    ,@ErrMsg			VARCHAR(200)        = '' OUTPUT
AS
/*
Created: 20200904Dwight.Tran
Task: Enhance NEX Request.
DB: NexHRM
Original: 

Revisions:   
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @EmployeeId			INT
        ,@Date				DATE
		,@NexdHour			NUMERIC (18, 1)
		,@OldNexMinute		INT
		,@VerifiedStatus	INT = 4
        ,@DocHandler		INT;

	IF OBJECT_ID('#OvertimeSheet') IS NOT NULL
	BEGIN
		DROP TABLE #OvertimeSheet;
	END;

    CREATE TABLE #OvertimeSheet (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[NexHour]      NUMERIC (18, 1)
    );

    IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			WHERE e.[Id] = @ModifiedBy
				AND e.Roles IN (3, 4) -- HR, SA
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only role HR have a permission to verify NEX request'

		RETURN;
	END;

    EXEC sp_xml_preparedocument @DocHandler OUTPUT, @OvertimeSheet;
     
    INSERT INTO #OvertimeSheet
    SELECT
        [EmployeeId]
        ,[Date]
        ,[NexHour]
    FROM OPENXML(@DocHandler, '/Root/OvertimeRecord', 1)
    WITH (
        [EmployeeId]    INT
        ,[Date]         DATE
        ,[NexHour]      NUMERIC (18, 1)
    );

    BEGIN TRY
        BEGIN TRANSACTION;

        WHILE (
            EXISTS (
                SELECT 1
                FROM #OvertimeSheet
            )
        )
        BEGIN
            SELECT TOP (1)
                @EmployeeId = [EmployeeId]
                ,@Date      = [Date]
				,@NexdHour	= [NexHour]
            FROM #OvertimeSheet;
			IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.OvertimeYearly WITH (NOLOCK)
					WHERE [Year] = YEAR(@date) AND [EmpId] = @EmployeeId
					)
				)
			BEGIN
				IF (NOT EXISTS (
						SELECT 1
						FROM dbo.OvertimeYearly WITH (NOLOCK)
						WHERE EmpId = @EmployeeId
							AND [Year] = (YEAR(@Date) - 1)
						))
				BEGIN
					INSERT INTO dbo.OvertimeYearly (
						EmpId
						,[Year]
						,[Minutes]
						,[ModifiedBy]
						,[ModifiedDate]
						)
					SELECT @EmployeeId
						,YEAR(@Date)
						,@NexdHour * 60
						,@ModifiedBy
						,dbo.fnc_GetClientDateTime()
				END
				ELSE
				BEGIN
					INSERT INTO dbo.OvertimeYearly (
						EmpId
						,[Year]
						,[Minutes]
						,[ModifiedBy]
						,[ModifiedDate]
						)
					SELECT @EmployeeId
						,YEAR(@Date)
						,ISNULL(prev.[Minutes], 0) + (@NexdHour * 60)
						,@ModifiedBy
						,dbo.fnc_GetClientDateTime()
					FROM (
						SELECT	EmpId
								,[Minutes]
						FROM dbo.OvertimeYearly WITH (NOLOCK)
						WHERE ([Year] = YEAR(@date) - 1) AND [EmpId] = @EmployeeId
						) AS prev
					WHERE NOT EXISTS (
							SELECT 1
							FROM dbo.OvertimeYearly WITH (NOLOCK)
							WHERE EmpId = prev.EmpId
								AND [Year] = YEAR(@Date)
							)
					END
			END

			IF EXISTS (
				SELECT 1 FROM [dbo].[OvertimeTrans]
				WHERE [EmpId] = @EmployeeId
					AND [OvertimeId] = @Id
					AND [Date] = @Date
			)
			BEGIN
				SELECT @OldNexMinute = [Minutes]
				FROM [dbo].[OvertimeTrans] WITH (NOLOCK)
				WHERE [EmpId] = @EmployeeId
					AND DATEDIFF(DAY, @Date, [Date]) = 0
					AND [OvertimeId] = @Id

				UPDATE [dbo].[OvertimeTrans] WITH (ROWLOCK, UPDLOCK)
				SET
					[Minutes]	= @NexdHour * 60
					,[ModifiedBy]   = @ModifiedBy
					,[ModifiedDate] = dbo.fnc_GetClientDateTime()
				WHERE [EmpId] = @EmployeeId
					AND DATEDIFF(DAY, @Date, [Date]) = 0
					AND [OvertimeId] = @Id;

				UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
				SET
					[Minutes]       = [Minutes] + @NexdHour * 60 - @OldNexMinute
					,[ModifiedBy]   = @ModifiedBy
					,[ModifiedDate] = dbo.fnc_GetClientDateTime()
				WHERE [EmpId] = @EmployeeId
					AND [Year] >= YEAR(@Date)
			END
			ELSE
			BEGIN
				INSERT INTO dbo.OvertimeTrans(
						OvertimeId
						,EmpId
						,[Date]
						,[Minutes]
						,[Type]
						,ModifiedBy
						,ModifiedDate
						)
						SELECT 
						@Id
						,@EmployeeId
						,@Date
						,@NexdHour * 60
						,1
						,@ModifiedBy
						,dbo.fnc_GetClientDateTime()

				UPDATE [dbo].[OvertimeYearly] WITH (ROWLOCK, UPDLOCK)
				SET
					[Minutes]       = [Minutes] + @NexdHour * 60
					,[ModifiedBy]   = @ModifiedBy
					,[ModifiedDate] = dbo.fnc_GetClientDateTime()
				WHERE [EmpId] = @EmployeeId
					AND [Year] >= YEAR(@Date)
			END
			UPDATE [dbo].[OvertimeDetail] WITH (ROWLOCK, UPDLOCK)
			SET [NexHour] = @NexdHour
			WHERE 
				[EmployeeId] = @EmployeeId
				AND DATEDIFF(DAY, @Date, [Date]) = 0
				AND [OvertimeId] = @Id

            DELETE TOP (1)
            FROM #OvertimeSheet
            WHERE
				EmployeeId = @EmployeeId
				AND DATEDIFF(DAY, [Date], @Date) = 0;
        END;

		UPDATE [dbo].[Overtime] WITH (ROWLOCK, UPDLOCK)
		SET
			[Status] = @VerifiedStatus,
			[ModifiedBy] = @ModifiedBy,
			[ModifiedDate] = dbo.fnc_GetClientDateTime()
		WHERE [OvertimeId] = @Id

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT
            @ErrCode = 1000
            ,@ErrMsg = 'Verify NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_VoidRequest] 
	@Id INT
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
	Created: 20200903Dwight.Tran
	Task: Enhance NEX Request.
	DB: NexHRM
	Original: 

	Revisions:  
		- 20250507@Henry.Dinh: update group id from 11 to 1081
	Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SET NOCOUNT ON;

    DECLARE
		@GroupManagerId		INT	= 1081
        ,@VoidStatus		INT = 5
        ,@ApprovedStatus	INT = 2
		,@VerifiedStatus	INT = 4;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
		)
	)
	BEGIN
		SELECT
			@ErrCode = 1
			,@ErrMsg = 'NEX request is not exist';

		RETURN;
	END;

	IF (
		NOT EXISTS (
			SELECT 1
			FROM [dbo].[Overtime] WITH (NOLOCK)
			WHERE [OvertimeId] = @Id
				AND ([Status] = @ApprovedStatus OR [Status] = @VerifiedStatus)
		)
	)
	BEGIN
		SELECT
			@ErrCode = 2
			,@ErrMsg = 'You only can void approved NEX request or verified NEX request';

		RETURN;
	END;

	IF (
        NOT EXISTS (
            SELECT 1
            FROM [dbo].[Employee] e WITH (NOLOCK)
			WHERE e.[Id] = @ModifiedBy
				AND e.Roles IN (3, 4) -- HR, SA
        )
    )
	BEGIN
		SELECT
			@ErrCode = 3
			,@ErrMsg = 'Only role HR have a permission to void NEX request'

		RETURN;
	END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.[Overtime] WITH (ROWLOCK, UPDLOCK)
        SET [Status] = @VoidStatus
            ,ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE OvertimeId = @Id;

		EXECUTE dbo.[Overtime_DeleteTransaction] @Id, @ModifiedBy;
        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        SELECT @ErrCode = 1000
            ,@ErrMsg = 'Void NEX request failed';

        ROLLBACK TRANSACTION;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Overtime_YearlyNexRequest] @year INT
	,@employeeId INT = NULL
	,@groupId INT = NULL
AS
/*
Created: 20200917-Lilac.Nguyen
Task: Get overtime yearly NEX request.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SELECT ott.EmployeeId
	,e.NickName
	,eg.Name AS GroupName
	,ott.[Hour]
	,ott.[Date]
	,ec.IsHoliday
	,ec.IsOffDay
	,(CASE WHEN ec.IsHoliday = 0 AND ec.IsOffDay = 0 THEN 1 ELSE 0 END) AS IsNormalDay
	INTO #OvertimeSummary
	FROM [dbo].[OvertimeDetail] AS ott WITH (NOLOCK)
		LEFT JOIN [dbo].EmployeeCalendar AS ec WITH (NOLOCK) ON ec.EmpId = ott.EmployeeId AND ec.WorkDate = ott.[Date]
		LEFT JOIN  Employee AS e WITH (NOLOCK) ON e.Id = ott.EmployeeId
		LEFT JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
	WHERE YEAR(ott.[Date]) = @year

	SELECT os.EmployeeId
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 1 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Jan'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 2 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Feb'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 3 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Mar'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 4 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Apr'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 5 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'May'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 6 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Jun'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 7 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Jul'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 8 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Aug'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 9 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Sep'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 10 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Otc'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 11 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Nov'
		,SUM(CASE 
			WHEN MONTH(os.[Date]) = 12 THEN ISNULL(os.[Hour], 0)
			ELSE 0
			END) 'Dec'
		,SUM([Hour]) 'Total'
	,os.IsHoliday
	,os.IsOffDay
	,os.IsNormalDay
	INTO #OvertimeTest
	 FROM  #OvertimeSummary AS os
	 INNER JOIN Employee AS e WITH (NOLOCK) ON e.Id = os.EmployeeId
	 GROUP BY os.EmployeeId, os.IsHoliday, os.IsOffDay, os.IsNormalDay

	IF OBJECT_ID('#OvertimeYearly') IS NOT NULL
	BEGIN
		DROP TABLE #OvertimeYearly
	END

	CREATE TABLE #OvertimeYearly (
		EmpId FLOAT
		,Code INT
		,NickName NVARCHAR(20)
		,FullName NVARCHAR(50)
		,GroupId INT
		,GroupCode VARCHAR(20)
		,GroupName NVARCHAR(50)
		,IsHoliday INT
		,IsOffDay INT
		,IsNormalDay INT
		,Jan FLOAT
		,Feb FLOAT
		,Mar FLOAT
		,Apr FLOAT
		,May FLOAT
		,Jun FLOAT
		,Jul FLOAT
		,Aug FLOAT
		,Sep FLOAT
		,Oct FLOAT
		,Nov FLOAT
		,[Dec] FLOAT
		,TotalHour FLOAT
		)
	INSERT INTO #OvertimeYearly (
		EmpId 
		,Code
		,NickName
		,FullName
		,GroupId
		,GroupCode
		,GroupName
		,IsHoliday
		,IsOffDay
		,IsNormalDay
		,Jan
		,Feb
		,Mar
		,Apr
		,May
		,Jun
		,Jul
		,Aug
		,Sep
		,Oct
		,Nov
		,[Dec]
		,TotalHour
		)
	SELECT e.Id
		,e.Code
		,e.NickName
		,e.FullName
		,eg.Id
		,eg.Code
		,eg.Name
		,ISNULL(o.IsHoliday, 0)
		,ISNULL(o.IsOffDay, 0)
		,ISNULL(o.IsNormalDay, 0)
		,ISNULL(o.Jan, 0)
		,ISNULL(o.Feb, 0)
		,ISNULL(o.Mar, 0)
		,ISNULL(o.Apr, 0)
		,ISNULL(o.May, 0)
		,ISNULL(o.Jun, 0)
		,ISNULL(o.Jul, 0)
		,ISNULL(o.Aug, 0)
		,ISNULL(o.Sep, 0)
		,ISNULL(o.Otc, 0)
		,ISNULL(o.Nov, 0)
		,ISNULL(o.[Dec], 0)
		,ISNULL(o.Total, 0)
	FROM  Employee AS e WITH (NOLOCK)
	LEFT JOIN #OvertimeTest AS o ON e.Id = o.EmployeeId
	JOIN EmployeeGroup AS eg WITH (NOLOCK) ON eg.Id = e.GroupId
	WHERE e.IsSpecialUser = 0
		AND  (
			@employeeId IS NULL
			OR e.Id = @employeeId
			)
		AND (
			@groupid IS NULL
			OR e.GroupId = @groupId
			)
		AND ISNULL(e.isDeleted, 0) = 0
	ORDER BY eg.[Rank]
		,eg.Code
		,e.Roles DESC
		,e.GroupDate

	SELECT Jan
		,Feb
		,Mar
		,Apr
		,May
		,Jun
		,Jul
		,Aug
		,Sep
		,Oct
		,Nov
		,[Dec]
		,TotalHour
		,IsHoliday
		,IsOffDay
		,IsNormalDay
		,EmpId AS EmployeeId
		,NickName
		,FullName
		,GroupId AS TeamId
		,GroupCode AS TeamCode
		,GroupName AS TeamName
	FROM #OvertimeYearly

DROP TABLE #OvertimeSummary

DROP TABLE #OvertimeTest

DROP TABLE #OvertimeYearly
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Promotion_Delete] @Id INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240104@Lilac.Nguyen
Task: delete a promotion 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    IF (
            NOT EXISTS (
                SELECT 1
                FROM HRM.dbo.EmployeeJobGrade WITH (NOLOCK)
                WHERE Id = @Id
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'The promotion does not exists '

        RETURN
    END    
    
   UPDATE HRM.dbo.EmployeeJobGrade
        SET IsDeleted = 1
			,ModifiedBy = @ModifiedBy
            ,ModifiedDate = dbo.fnc_GetClientDateTime()
        WHERE Id = @Id

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Promotion_Get] @FromDate DATETIME
    ,@ToDate DATETIME
    ,@GroupId INT = NULL
	,@EmpId INT = NULL
AS
/*
Created: 20240103@Lilac.Nguyen
Task: Get promotions.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT ej.Id
	,e.FullName
	,e.NickName
	,eg.Code AS TeamCode
	,eg.Name AS TeamName
	,jg.Name AS CurrentGrade
	,jg2.Name AS PromotedGrade
	,ej.AffectedDate
	,ej.Note

	FROM HRM.dbo.EmployeeJobGrade AS ej WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ej.EmpId
		INNER JOIN dbo.EmployeeGroup AS eg WITH (NOLOCK) ON e.GroupId = eg.Id
		INNER JOIN HRM.dbo.JobGrade AS jg WITH (NOLOCK) ON jg.JobGradeId = ej.OldJobGradeId
		INNER JOIN HRM.dbo.JobGrade AS jg2 WITH (NOLOCK) ON jg2.JobGradeId = ej.JobGradeId
	WHERE (@GroupId IS NULL OR e.GroupId = @GroupId)
		AND (@EmpId IS NULL OR ej.EmpId = @EmpId)
		AND DATEDIFF(DAY, @FromDate, ej.AffectedDate) >= 0
		AND DATEDIFF(DAY, @ToDate, ej.AffectedDate) <= 0
		AND (ej.IsDeleted = 0 OR ej.IsDeleted IS NULL)
		AND (ej.IsPromotion = 1)
       
	ORDER BY ej.AffectedDate DESC
            
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Promotion_GetById]
    @Id INT
AS
/*
Created: 20240103@Lilac.Nguyen
Task: Get Promotion by Id.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT ej.Id
	,ej.EmpId AS EmployeeId
	,e.NickName
	,e.FullName
	,ej.JobGradeId
	,ej.OldJobGradeId
	,ej.AffectedDate
	,ej.Note
    FROM HRM.dbo.EmployeeJobGrade AS ej WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = ej.EmpId
    WHERE ej.Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Promotion_Insert] 
    @EmpId INT
    ,@JobGradeId INT
	,@OldJobGradeId INT
    ,@EffectiveDate DATETIME
	,@Note NVARCHAR(1000) = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240104@Lilac.Nguyen
Task: Insert Promotion
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

	IF (
            EXISTS (
                SELECT 1
                FROM HRM.dbo.EmployeeJobGrade WITH (NOLOCK)
                WHERE EmpId = @EmpId 
				AND ((OldJobGradeId = @OldJobGradeId AND JobGradeId =  @JobGradeId ) OR @EffectiveDate = AffectedDate)
				AND (IsDeleted = 0 OR IsDeleted IS NULL)
				AND IsPromotion = 1
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Promotion has already exists.'

        RETURN
    END

	IF (dbo.fnc_GetClientDateTime() >= @EffectiveDate)
    BEGIN
        UPDATE HRM.dbo.EmployeeJobGrade WITH (ROWLOCK, UPDLOCK)
		SET 
			IsCurrentUsed = 0
		WHERE EmpId = @EmpId  AND dbo.fnc_GetClientDateTime() >= AffectedDate AND  @EffectiveDate > AffectedDate
    END

    INSERT INTO HRM.dbo.EmployeeJobGrade(
		EmpId
		,JobGradeId
		,OldJobGradeId
		,AffectedDate
		,ModifiedBy
		,ModifiedDate
		,IsCurrentUsed
		,Note
		,IsDeleted
		,IsPromotion
        )

    VALUES (
        @EmpId
		,@JobGradeId
		,@OldJobGradeId
		,@EffectiveDate
		,@ModifiedBy
		,dbo.fnc_GetClientDateTime()
		,CASE
			WHEN @EffectiveDate > dbo.fnc_GetClientDateTime() THEN 0
			ELSE 1
			END
		,@Note
		,NULL
		,1
        )

    SELECT @ErrCode = 0, @ErrMsg = ''
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Promotion_Update] 
    @Id INT
	,@EmpId INT
    ,@JobGradeId INT
	,@OldJobGradeId INT
    ,@EffectiveDate DATETIME
	,@Note NVARCHAR(1000) = NULL
    ,@ModifiedBy INT
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240104@Lilac.Nguyen
Task: Update a promotion
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;
	IF (
            EXISTS (
                SELECT 1
                FROM HRM.dbo.EmployeeJobGrade WITH (NOLOCK)
                WHERE EmpId = @EmpId 
				AND ((OldJobGradeId = @OldJobGradeId AND JobGradeId =  @JobGradeId ) OR @EffectiveDate = AffectedDate)
				AND (IsDeleted = 0 OR IsDeleted IS NULL)
				AND IsPromotion = 1
				AND Id <> @Id
                )
            )
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Promotion has already exists.'

        RETURN
    END

	
	IF (dbo.fnc_GetClientDateTime() >= @EffectiveDate)
    BEGIN
        UPDATE HRM.dbo.EmployeeJobGrade WITH (ROWLOCK, UPDLOCK)
		SET 
			IsCurrentUsed = 0
		WHERE EmpId = @EmpId AND Id <> @Id AND dbo.fnc_GetClientDateTime() >= AffectedDate AND  @EffectiveDate > AffectedDate
    END

    UPDATE HRM.dbo.EmployeeJobGrade WITH (ROWLOCK, UPDLOCK)
    SET 
		JobGradeId = @JobGradeId
		,OldJobGradeId = @OldJobGradeId
		,AffectedDate = @EffectiveDate
		,IsCurrentUsed = CASE
							WHEN @EffectiveDate > dbo.fnc_GetClientDateTime() THEN 0
							ELSE 1
							END
		,Note = @Note
        ,ModifiedBy = @ModifiedBy
        ,ModifiedDate = dbo.fnc_GetClientDateTime()
    WHERE Id = @Id
  
    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update promotion failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[RedmineSpendTimeWriteLog_Insert] 
    @EmpId INT
    ,@IssueId INT
    ,@SpentOn DATE
    ,@Hours FLOAT
    ,@Comments NVARCHAR(255) = null
	,@Activity INT
    ,@CreatedDate DATETIME
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180620@Oliver.Dang
Task: Insert log when log time redmine from HRM
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.RedmineSpendTimeLog(
        [EmpId]
        ,[IssueId]
        ,[SpentOn]
        ,[Hours]
        ,[Comments]
        ,[Activity]
        ,[CreatedDate]
        )
    VALUES (
        @EmpId
        ,@IssueId
        ,@SpentOn
        ,@Hours
        ,@Comments
		,@Activity
        ,@CreatedDate
        )

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Insert log failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_CompanySpaceDashBoard] @EmpId INT
AS
/*
Created: 20150817@Quinn.Vo
Task: Show dashboard info for employee.
DB: NexHR
Original: 

Teddy: add list employee work from home

Revisions:    
Param's Explanation:
- Type: 1 Leave, 2: Check In/Out, 3: Missing Check In/Out, 4: Late In/Early Out, 5: Absent, 6: Total Working Hours, 7: Annual Leave Balance
*/
BEGIN
    DECLARE @Today DATETIME
        ,@Yesterday DATETIME
    SET @Today = dbo.fnc_GetClientDateTime()

    SELECT @Yesterday = DATEADD(D, - 1, @Today)

	SELECT WorkDate, IsCurrent, IsWorkFromHome, TypeId, SUM(LeaveDay) AS LeaveDay, EmpId, NickName, FullName
	FROM
	(
    SELECT cl.WorkDate
        ,(CASE 
            WHEN DATEDIFF(D, cl.WorkDate, @Today) = 0 THEN 1
            ELSE 0 END
            ) AS IsCurrent
		,0 AS IsWorkFromHome
        ,1 AS TypeId
        ,CASE 
            WHEN wk.TotalDays >= 1 THEN 1
            ELSE 0.5
            END AS LeaveDay
        ,wk.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e  WITH (NOLOCK) ON wk.EmpId = e.Id
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON DATEDIFF(D, wk.FromDate, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, wk.ToDate) >= 0
        AND wk.EmpId = cl.EmpId
        AND cl.IsOffDay <> 1
        AND cl.IsHoliday <> 1
    WHERE wk.RequestType = 6
        AND wk.[Status] IN (1, 2)
        AND DATEDIFF(D, @Yesterday, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, @Today) >= 0
        AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
    ) AS a
	GROUP BY WorkDate, IsWorkFromHome, IsCurrent, TypeId, EmpId, NickName, FullName

    UNION
    SELECT wk.FromDate AS WorkDate
        ,(CASE 
            WHEN DATEDIFF(D, wk.FromDate, @Today) = 0 THEN 1
            ELSE 0 END
            ) AS IsCurrent
		,0 AS IsWorkFromHome
        ,2 AS TypeId
        ,0 AS LeaveDay
        ,wk.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = wk.EmpId
    WHERE wk.RequestType IN (1, 2)
        AND wk.[Status] IN (1, 2)
        AND DATEDIFF(D, @Yesterday, wk.FromDate) >= 0
        AND DATEDIFF(D, wk.FromDate, @Today) >= 0
        AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
    UNION
    SELECT cl.WorkDate
        ,(CASE 
            WHEN DATEDIFF(D, cl.WorkDate, @Today) = 0 THEN 1
            ELSE 0 END
            ) AS IsCurrent
		,0 AS IsWorkFromHome
        ,CASE
            WHEN (
                    (
                        att.LateIn > 0
                        OR att.EarlyOut > 0
                        )
                    AND e.Id NOT IN (88, 89, 123, 134, 153)
                    ) THEN 4
            ELSE CASE 
                    WHEN (
                            (
                                ISNULL(Absence, 0) = 0
                                AND att.InFixed IS NULL
                                AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
                                )
                            OR (
                                ISNULL(Absence, 0) = 0
                                AND att.OutFixed IS NULL
                                AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckOutTime), @Today) > 0
                                )
                            ) THEN 3
                    ELSE CASE 
                            WHEN (
                                    ISNULL(att.Absence, 0) = 1
                                    AND cl.IsOffDay = 0
                                    AND cl.IsHoliday = 0
                                    AND ISNULL(att.LeaveDay, 0) <> 1
                                    AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
                                    ) THEN 5
                            ELSE 0
                            END
                    END
            END AS TypeId
        ,0 AS LeaveDay
        ,att.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.Id
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON cl.EmpId = att.EmpId
        AND cl.Workdate = att.AttendanceDate
        LEFT JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON cl.EmpId = wk.EmpId
        AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
        AND wk.RequestType IN (1, 2)
        AND wk.[Status] = 1
        AND (
            (
                att.InFixed IS NULL
                AND wk.RequestType = 1
                )
            OR (
                att.OutFixed IS NULL
                AND wk.RequestType = 2
                )
            )
        LEFT JOIN dbo.WorkingTimeRequest AS wkl WITH (NOLOCK) ON cl.EmpId = wkl.EmpId
        AND DATEDIFF(D, wkl.FromDate, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, wkl.ToDate) >= 0
        AND wkl.RequestType = 6
        AND wkl.[Status] IN (1, 2)
        LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.Id = att.ShiftId
    WHERE DATEDIFF(D, @Yesterday, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, @Today) >= 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
        AND (
            (
                (
                    att.LateIn > 0
                    OR att.EarlyOut > 0
                    )
                AND e.Id NOT IN (88, 89, 123, 134, 153)
                )
            OR (
                ISNULL(att.Absence, 0) = 1
                AND cl.IsOffDay = 0
                AND cl.IsHoliday = 0
                AND ISNULL(att.LeaveDay, 0) <> 1
                AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
                AND wk.Id IS NULL
                AND (
                    wkl.Id IS NULL
                    OR (
                        wkl.Id IS NOT NULL
                        AND wkl.TotalDays = 0.5
                        AND DATEDIFF(HH, @Today, (att.AttendanceDate + ' ' + s.EndCheckOutTime)) < 4
                        )
                    )
                )
            OR (
                ISNULL(Absence, 0) = 0
                AND att.InFixed IS NULL
                AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
                AND (
                    wk.Id IS NULL
                    OR wk.RequestType = 2
                    )
                )
            OR (
                ISNULL(Absence, 0) = 0
                AND att.OutFixed IS NULL
                AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckOutTime), @Today) > 0
                AND (
                    wk.Id IS NULL
                    OR wk.RequestType = 1
                    )
                )
            )
	UNION
    SELECT cl.WorkDate
        ,(CASE 
            WHEN DATEDIFF(D, cl.WorkDate, @Today) = 0 THEN 1
            ELSE 0 END
            ) AS IsCurrent
		,cl.IsWorkFromHome AS IsWorkFromHome
        ,0 AS TypeId
        ,0 AS LeaveDay
        ,cl.EmpId
        ,e.NickName
        ,e.FullName
    FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = cl.EmpId
    WHERE
        DATEDIFF(D, @Yesterday, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, @Today) >= 0
        AND ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND ISNULL(e.IsSpecialUser, 0) = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_EmployeeDashBoard] @EmpId INT
AS
/*
Created: 20150817@Quinn.Vo
Task: Show dashboard info for employee.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
- Type: 1 Leave, 2: Check In/Out, 3: Missing Check In/Out, 4: Late In/Early Out, 5: Absent, 6: Total Working Hours, 7: Annual Leave Balance
*/
BEGIN
	DECLARE @CurrentMonth DATETIME
		,@LastMonth DATETIME
		,@Today DATETIME
		,@Yesterday DATETIME
		,@CheckInOutCurrentMonth INT
		,@CheckInOutLastMonth INT
		,@MissingCheckInOutCurrentMonth INT
		,@MissingCheckInOutLastMonth INT
		,@LateInEarlyOutCurrentMonth INT
		,@LateInEarlyOutLastMonth INT
		,@AbsentCurrentMonth INT
		,@AbsentLastMonth INT
		,@AnnualLeaveBalancePending NUMERIC(5, 1)
		,@AnnualLeaveBalance NUMERIC(5, 1)
		,@TotalWorkingHoursToday DECIMAL(18, 2)
		,@TotalWorkingHoursCurrentMonth DECIMAL(18, 2)
		,@TotalWorkingHoursLastMonth DECIMAL(18, 2)
		,@NumberWorkingDayCurrentMonth DECIMAL(18, 2)
		,@NumberWorkingDayLastMonth DECIMAL(18, 2)
		,@LeaveCurrentMonth DECIMAL(18, 1)
		,@LeaveLastMonth DECIMAL(18, 1)
		,@CheckInTime DATETIME
		,@CheckOutTime DATETIME
		,@MinuteLateInEarlyOutCurrentMonth int
		,@TimesLateInEarlyOutCurrentMonth int

	SET @Today = dbo.fnc_GetClientDateTime()
	SELECT @CurrentMonth = @Today
		,@LastMonth = DATEADD(M, - 1, @Today)
		,@Yesterday = DATEADD(D, - 1, @Today)

	SELECT @AnnualLeaveBalance = SUM(CreditDays) - SUM(DeductDays)
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE EmpId = @EmpId 
		AND ISNULL(Deleted, 0) = 0  
		AND YEAR(FromDate) = YEAR(@Today)

	SELECT @AnnualLeaveBalancePending = SUM(ISNULL(wk.TotalDays, 0))
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON e.Id = wk.EmpId
	WHERE e.Id = @EmpId
		AND wk.RequestType = 6
		AND wk.[Status] = 1
		AND wk.LeaveTypeID IN (2, 30)
		-- AND YEAR(wk.FromDate) = YEAR(dbo.fnc_GetClientDateTime())
	GROUP BY e.Id

	SELECT @TotalWorkingHoursCurrentMonth = SUM(CASE 
				WHEN DATEDIFF(M, att.AttendanceDate, @CurrentMonth) = 0 THEN TotalWorkHours
				ELSE 0
				END)
		,@TotalWorkingHoursLastMonth = SUM(CASE 
				WHEN DATEDIFF(M, att.AttendanceDate, @LastMonth) = 0 THEN TotalWorkHours
				ELSE 0
				END)
		,@NumberWorkingDayCurrentMonth = SUM(CASE 
				WHEN (
						DATEDIFF(M, att.AttendanceDate, @CurrentMonth) = 0
						AND ISNULL(att.TotalWorkHours, 0) <> 0
						) THEN (CASE 
						WHEN att.LeaveDay = 0.5 THEN 0.5
						ELSE 1 END
						)
				ELSE 0
				END)
		,@NumberWorkingDayLastMonth = SUM(CASE 
				WHEN (
						DATEDIFF(M, att.AttendanceDate, @LastMonth) = 0
						AND ISNULL(att.TotalWorkHours, 0) <> 0
						) THEN (CASE 
						WHEN att.LeaveDay = 0.5 THEN 0.5
						ELSE 1 END
						)
				ELSE 0
				END)
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
	INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON att.EmpId = cl.EmpId
		AND att.AttendanceDate = cl.WorkDate
	WHERE att.EmpId = @EmpId
		AND DATEDIFF(M, @LastMonth, att.AttendanceDate) > = 0
		AND DATEDIFF(M, att.AttendanceDate, @CurrentMonth) > = 0
		

	SELECT @CheckInTime = CASE WHEN cl.RequestedCheckinTime IS NULL THEN att.InFixed ELSE cl.RequestedCheckinTime END
		,@CheckOutTime = CASE WHEN cl.RequestedCheckoutTime IS NULL THEN att.OutFixed ELSE cl.RequestedCheckoutTime END
	FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON att.EmpId = cl.EmpId
		AND att.AttendanceDate = cl.WorkDate
	WHERE cl.EmpId = @EmpId
		AND DATEDIFF(D, cl.WorkDate, @Today) = 0

	EXEC [EmployeeAttendance_TotalWorkingHoursToday] @EmpId, @TotalWorkHoursToday = @TotalWorkingHoursToday OUTPUT

     -----Get Late in/Early out current month (hour/time) for dashboard-------
	SELECT @MinuteLateInEarlyOutCurrentMonth = SUM(tr.Minutes), @TimesLateInEarlyOutCurrentMonth = COUNT(tr.Minutes)
	FROM dbo.LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE --MONTH(WorkDate) = MONTH(dbo.fnc_GetClientDateTime()) 
		DATEDIFF(MM, WorkDate, dbo.fnc_GetClientDateTime()) = 0
		AND tr.Type <> 0
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
	GROUP BY EmpId;
	
	SELECT ISNULL(@AnnualLeaveBalance, 0) - ISNULL(@AnnualLeaveBalancePending, 0) AS AnnualLeaveBalance
		,@TotalWorkingHoursToday AS TotalWorkingHoursToday
		,@TotalWorkingHoursCurrentMonth AS TotalWorkingHoursCurrentMonth
		,@TotalWorkingHoursLastMonth AS TotalWorkingHoursLastMonth
		,@NumberWorkingDayCurrentMonth AS NumberWorkingDayCurrentMonth
		,@NumberWorkingDayLastMonth AS NumberWorkingDayLastMonth
		,@CheckInTime AS CheckInTime
		,@CheckOutTime AS CheckOutTime
		,@MinuteLateInEarlyOutCurrentMonth AS MinutesLateInEarlyOutCurrentMonth
		,@TimesLateInEarlyOutCurrentMonth AS TimesLateInEarlyOutCurrentMonth
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_EmployeeSpaceDashBoard] @EmpId INT
AS
/*
Created: 20150817@Quinn.Vo
Task: Show dashboard info for employee.
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
- Type: 1 Leave, 2: Check In/Out, 3: Missing Check In/Out, 4: Late In/Early Out, 5: Absent, 6: Total Working Hours, 7: Annual Leave Balance
*/
BEGIN
	DECLARE @CurrentMonth DATETIME
		,@LastMonth DATETIME
		,@Today DATETIME

	SET @Today = dbo.fnc_GetClientDateTime()

	SELECT @CurrentMonth = @Today
		,@LastMonth = DATEADD(M, - 1, @Today)

	SELECT wk.FromDate AS WorkDate
		,(CASE 
			WHEN DATEDIFF(M, wk.FromDate, @CurrentMonth) = 0 THEN 1
			ELSE 0 END
			) AS IsCurrent
		,2 AS TypeId
		,0 AS LeaveDay
		,wk.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = wk.EmpId
	WHERE wk.EmpId = @EmpId
		AND wk.RequestType IN (1, 2)
		AND wk.[Status] IN (1, 2)
		AND DATEDIFF(M, @LastMonth, wk.FromDate) > = 0
		AND DATEDIFF(M, wk.FromDate, @CurrentMonth) > = 0
		AND EXISTS (
			SELECT 1 
			FROM dbo.EmployeeCalendar WITH (NOLOCK) 
			WHERE DATEDIFF(dd, WorkDate, wk.FromDate) = 0
			AND IsHoliday = 0
			AND IsOffDay = 0
		)
	
	UNION
	SELECT cl.WorkDate
		,(CASE 
			WHEN DATEDIFF(M, cl.WorkDate, @CurrentMonth) = 0 THEN 1
			ELSE 0 END
			) AS IsCurrent
		,1 AS TypeId
		,(CASE 
			WHEN wk.TotalDays = 0.5 THEN 0.5
			ELSE 1 END
			) AS LeaveDay
		,wk.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = wk.EmpId
		INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON DATEDIFF(D, wk.FromDate, cl.WorkDate) >= 0
		AND DATEDIFF(D, cl.WorkDate, wk.ToDate) >= 0
		AND wk.EmpId = cl.EmpId
		AND cl.IsOffDay <> 1
		AND cl.IsHoliday <> 1
	WHERE wk.EmpId = @EmpId
		AND wk.RequestType = 6
		AND wk.[Status] IN (1, 2)
		AND DATEDIFF(M, @LastMonth, cl.WorkDate) >= 0
		AND DATEDIFF(M, cl.WorkDate, @CurrentMonth) >= 0
	--GROUP BY wk.EmpId
	
	UNION
	SELECT cl.WorkDate
		,(CASE 
			WHEN DATEDIFF(M, cl.WorkDate, @CurrentMonth) = 0 THEN 1
			ELSE 0 END
			) AS IsCurrent
		,CASE 
			WHEN (
					(
						att.LateIn > 0
						OR att.EarlyOut > 0
						)
					-- AND e.Id NOT IN (88, 89, 123, 134, 153)
					) THEN 4
			ELSE CASE 
					WHEN (
							(
								ISNULL(Absence, 0) = 0
								AND att.InFixed IS NULL
								AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
								)
							OR (
								ISNULL(Absence, 0) = 0
								AND att.OutFixed IS NULL
								AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckOutTime), @Today) > 0
								)
							) THEN 3
					ELSE CASE 
							WHEN (
									ISNULL(att.Absence, 0) = 1
									AND cl.IsOffDay = 0
									AND cl.IsHoliday = 0
									AND ISNULL(att.LeaveDay, 0) <> 1
									AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
									) THEN 5
							ELSE 0
							END
					END
			END AS TypeId
		,0 AS LeaveDay
		,att.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e ON att.EmpId = e.Id
		INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON cl.EmpId = att.EmpId
		AND cl.Workdate = att.AttendanceDate
		LEFT JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON cl.EmpId = wk.EmpId
		AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
		AND wk.RequestType IN (1, 2)
		AND wk.[Status] = 1
		AND (
			(
				att.InFixed IS NULL
				AND wk.RequestType = 1
				)
			OR (
				att.OutFixed IS NULL
				AND wk.RequestType = 2
				)
			)
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.Id = att.ShiftId
	WHERE att.EmpId = @EmpId
		AND DATEDIFF(M, @LastMonth, cl.WorkDate) >= 0
		AND DATEDIFF(M, cl.WorkDate, @CurrentMonth) >= 0
		AND (
			(
				(
					att.LateIn > 0
					OR att.EarlyOut > 0
					)
				-- AND e.Id NOT IN (88, 89, 123, 134, 153)
				)
			OR (
				ISNULL(att.Absence, 0) = 1
				AND cl.IsOffDay = 0
				AND cl.IsHoliday = 0
				AND ISNULL(att.LeaveDay, 0) <> 1
				AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
				AND wk.Id IS NULL
				)
			OR (
				ISNULL(Absence, 0) = 0
				AND att.InFixed IS NULL
				AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckInTime), @Today) > 0
				AND (
					wk.Id IS NULL
					OR wk.RequestType = 2
					)
				)
			OR (
				ISNULL(Absence, 0) = 0
				AND att.OutFixed IS NULL
				AND DATEDIFF(MI, (att.AttendanceDate + ' ' + s.EndCheckOutTime), @Today) > 0
				AND (
					wk.Id IS NULL
					OR wk.RequestType = 1
					)
				)
			)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_GetEmployeeWorkLoad]
	@FromDate DATETIME
	,@ToDate DATETIME
	,@GroupId INT
	,@Debug BIT = 0
AS
/*
Created: 20160321@Quinn.Vo
Task: Get effort of every member in a team.
DB: NexEPA
Original: 

Revisions:  
	20200120@Merlin.Le: Order by employee rank and code
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT @ToDate = DATEADD(dd, -1, @ToDate)

	IF OBJECT_ID('#TblTeamWorkHours') IS NOT NULL
	BEGIN
		DROP TABLE #TblTeamWorkHours
	END

	CREATE TABLE #TblTeamWorkHours (
		EmpId INT
		,WorkHour INT
	)

	IF OBJECT_ID('#TblNickNames') IS NOT NULL
	BEGIN
		DROP TABLE #TblNickNames
	END
	
	CREATE TABLE #TblNickNames (EmpId INT, NickName VARCHAR(100))
	
    INSERT INTO #TblNickNames(EmpId, NickName)
	SELECT e.Id, REPLACE(e.DomainUsername,'spvn\','')
	FROM dbo.Employee AS e
	WHERE e.GroupId = @GroupId
	AND e.Id NOT IN (SELECT ApproverId FROM dbo.GroupSupervisor WHERE GroupId = @GroupId)

	-- get work hours from hrm
	INSERT INTO #TblTeamWorkHours
	SELECT
		e.Id
		,SUM(CASE 
				WHEN cl.LeaveDay = 0.5 THEN 0.5
				ELSE 1
				END) * 8
	FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON cl.EmpId = e.Id
	WHERE e.GroupId = @GroupId
		AND e.DomainUsername IS NOT NULL
		AND e.Id NOT IN (SELECT ApproverId FROM dbo.GroupSupervisor WHERE GroupId = @GroupId)
		AND cl.WorkDate BETWEEN @FromDate AND @ToDate
		AND cl.IsHoliday = 0
		AND cl.IsOffDay = 0
		AND (
			cl.IsLeaveDay = 0
			OR (
				cl.IsLeaveDay = 1
				AND cl.LeaveDay = 0.5
				)
			)
	GROUP BY e.Id
	
	--Member Workload
	SELECT 
		0.0 as PercentWorkload,
		h.WorkHour,
		e.Id,
		e.Code,
		e.NickName,
		e.FullName,
		e.DomainUsername
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN #TblTeamWorkHours AS h ON e.Id = h.EmpId
        INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID = e.GroupId
    WHERE e.GroupId = @GroupId
	AND e.Id NOT IN (SELECT ApproverId FROM dbo.GroupSupervisor WHERE GroupId = @GroupId)
	AND e.Enabled = 1
	AND ISNULL(e.isDeleted, 0) = 0
	ORDER BY
        e.Roles DESC
        ,e.GroupDate

	DROP TABLE #TblNickNames
	DROP TABLE #TblTeamWorkHours
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_GetHeadCountByYear]
	 @startDateOfYear DATETIME
    ,@endDateOfYear DATETIME
AS
/*
Created: 2018/05/22@Oliver.Dang
Task: get head count of year
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SELECT g.Id AS GroupId
		  ,g.Code AS GroupCode
		  ,g.Name AS GroupName
		  ,SUM( CASE
				WHEN (e.JoinedDate <= @startDateOfYear) AND ((e.[Enabled] = 1 AND e.isDeleted = 0) OR e.ResignedDate >= @startDateOfYear) 
				THEN 1
				ELSE 0
				END) AS EmployeeAtJanuary
		  ,SUM( CASE
				WHEN e.JoinedDate BETWEEN @startDateOfYear AND @endDateOfYear
				THEN 1
				ELSE 0
				END) AS NewHire
		  ,SUM( CASE
				WHEN e.ResignedDate BETWEEN @startDateOfYear AND @endDateOfYear
				THEN 1
				ELSE 0
				END) AS Resigned
		 ,SUM( CASE
				WHEN (e.JoinedDate <= @endDateOfYear) AND ((e.[Enabled] = 1 AND e.isDeleted = 0) OR e.ResignedDate > @endDateOfYear)
				THEN 1
				ELSE 0
				END) AS EmployeeAtDecember
		 ,(SELECT COUNT(h.Id) 
			FROM dbo.HistoryWork as h
				LEFT JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
			WHERE hd.GroupId = g.Id AND h.AffectedDate BETWEEN @startDateOfYear AND @endDateOfYear) AS [JoinTeam]
		 ,(SELECT COUNT(m.Id) 
			FROM (SELECT h.Id, h.EmpId, h.AffectedDate, hd.*
					FROM dbo.HistoryWork as h
					LEFT JOIN dbo.HistoryWorkDetail AS hd WITH (NOLOCK) ON hd.HistoryWorkId = h.Id
					WHERE h.AffectedDate NOT IN (SELECT TOP 1 AffectedDate 
													FROM dbo.HistoryWork
													WHERE EmpId = h.EmpId
													ORDER BY AffectedDate DESC)) AS m
			WHERE m.GroupId = g.Id AND m.AffectedDate BETWEEN @startDateOfYear AND @endDateOfYear)	AS [MoveTeam]

	FROM [dbo].EmployeeGroup As g WITH (NOLOCK)		
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON e.GroupId = g.Id
	WHERE ISNULL(g.Deleted, 0) = 0
		AND e.IsSpecialUser = 0
	GROUP BY g.Id
			,g.Name
			,g.Code
			,g.[Rank]
	ORDER BY g.[Rank]
			,g.Code
			,g.Name
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_GetLateInEarlyOut] @Year INT
	,@GroupId INT = NULL
	,@EmpId INT = NULL
AS
/*
	Created: 20150805@Quinn.Vo
	Task: Get late in/ early out summary report
	DB: NexHRM
	Original: 

	Revisions:
		- 20160129@Razor.Nguyen: OT Feature
		- 20250411@Henry.Dinh: Ignore CORE Team
	Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('#tblAnnualLeaveBalance') IS NOT NULL
	BEGIN
		DROP TABLE #tblAnnualLeaveBalance;
	END;

	CREATE TABLE #tblAnnualLeaveBalance (
		EmpId INT
		,AnnualLeaveBalance NUMERIC(10, 1)
		);

	IF OBJECT_ID('#PendingAnnualLeaveRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingAnnualLeaveRequest;
	END;

	CREATE TABLE #PendingAnnualLeaveRequest (
		EmpId INT
		,TotalDays NUMERIC(10, 1)
		);

	IF OBJECT_ID('#PreviousYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #PreviousYearBalance;
	END;

	CREATE TABLE #PreviousYearBalance (
		EmpId INT
		,[Minutes] INT
		,ConversionValue INT
		);

	IF OBJECT_ID('#CurrentYearBalance') IS NOT NULL
	BEGIN
		DROP TABLE #CurrentYearBalance;
	END;

	CREATE TABLE #CurrentYearBalance (
		EmpId INT
		,[Minutes] INT
		,Overtime INT
		);

	IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
	BEGIN
		DROP TABLE #LateInEarlyOutSummary;
	END;

	CREATE TABLE #LateInEarlyOutSummary (
		EmpId INT
		,Jan INT
		,Feb INT
		,Mar INT
		,Apr INT
		,May INT
		,Jun INT
		,Jul INT
		,Aug INT
		,Sep INT
		,Oct INT
		,Nov INT
		,[Dec] INT
		,TimesInJan INT
		,TimesInFeb INT
		,TimesInMar INT
		,TimesInApr INT
		,TimesInMay INT
		,TimesInJun INT
		,TimesInJul INT
		,TimesInAug INT
		,TimesInSep INT
		,TimesInOct INT
		,TimesInNov INT
		,TimesInDec INT
		,TotalMinutes INT
		,TotalTimes INT
		,TotalMinutesLeaveDays INT
		);

	INSERT INTO #PreviousYearBalance (
		EmpId
		,[Minutes]
		,ConversionValue
		)
	SELECT e.Id
		,(
			ISNULL(la.[Minutes], 0) - CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
		,CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0)
			END
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = (@Year - 1)
		LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = (@Year - 1)
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			);

	INSERT INTO #CurrentYearBalance (
		EmpId
		,[Minutes]
		,Overtime
		)
	SELECT e.Id
		,CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN (ISNULL(la.[Minutes], 0) - ISNULL(oty.[Minutes], 0))
			ELSE 0
			END
		,(CASE 
			WHEN ISNULL(la.[Minutes], 0) > ISNULL(oty.[Minutes], 0) THEN ISNULL(oty.[Minutes], 0)
			ELSE ISNULL(la.[Minutes], 0) END
			)
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.LateInEarlyOutYearly AS la WITH (NOLOCK) ON e.Id = la.EmpId
		AND la.[Year] = @Year
		LEFT JOIN #PreviousYearBalance AS prev ON e.Id = prev.EmpId
		LEFT JOIN dbo.OvertimeYearly AS oty WITH (NOLOCK) ON e.Id = oty.EmpId
		AND oty.[Year] = @Year
	WHERE e.IsSpecialUser = 0
		AND ISNULL(e.isDeleted, 0) = 0
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			);

	INSERT INTO #LateInEarlyOutSummary (
		EmpId
		,Jan
		,Feb
		,Mar
		,Apr
		,May
		,Jun
		,Jul
		,Aug
		,Sep
		,Oct
		,Nov
		,DEC
		,TimesInJan
		,TimesInFeb
		,TimesInMar
		,TimesInApr
		,TimesInMay
		,TimesInJun
		,TimesInJul
		,TimesInAug
		,TimesInSep
		,TimesInOct
		,TimesInNov
		,TimesInDec
		,TotalMinutes
		,TotalTimes
		,TotalMinutesLeaveDays
		)
	SELECT EmpId
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 1
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 2
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 3
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 4
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 5
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 6
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 7
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 8
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 9
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 10
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 11
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 12
						AND [Type] <> 0
						) THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 1
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 2
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 3
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 4
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 5
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 6
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 7
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 8
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 9
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 10
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 11
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN (
						MONTH(WorkDate) = 12
						AND [Type] <> 0
						) THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0 THEN [Minutes]
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] <> 0 THEN 1
				ELSE 0
				END)
		,SUM(CASE 
				WHEN [Type] = 0 THEN [Minutes]
				ELSE 0
				END)
	FROM dbo.LateInEarlyOutTrans AS tr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
	WHERE e.IsSpecialUser = 0
		AND YEAR(WorkDate) = @Year
		AND (
			@EmpId IS NULL
			OR e.Id = @EmpId
			)
		AND (
			@GroupId IS NULL
			OR e.GroupId = @GroupId
			)
	GROUP BY EmpId;

	INSERT INTO #PendingAnnualLeaveRequest (
		EmpId
		,TotalDays
		)
	SELECT rq.EmpId
		,SUM(rq.TotalDays)
	FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
		LEFT JOIN dbo.LeaveType AS ty WITH (NOLOCK) ON rq.LeaveTypeID = ty.Id
	WHERE [Status] = 1
		AND RequestType = 6
		AND ty.IsALDeduction = 1
	GROUP BY rq.EmpId;

	INSERT INTO #tblAnnualLeaveBalance(EmpId, AnnualLeaveBalance)
	SELECT lt.EmpId, SUM(lt.CreditDays) - SUM(lt.DeductDays)
	FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON lt.EmpId = e.Id
	WHERE e.Enabled = 1 
	AND e.IsDeleted = 0
	AND YEAR(lt.FromDate) = @Year
	AND lt.Deleted = 0
	GROUP BY lt.EmpId

	SELECT dt.Jan
		,dt.Feb
		,dt.Mar
		,dt.Apr
		,dt.May
		,dt.Jun
		,dt.Jul
		,dt.Aug
		,dt.Sep
		,dt.Oct
		,dt.Nov
		,dt.[Dec]
		,dt.TimesInJan
		,dt.TimesInFeb
		,dt.TimesInMar
		,dt.TimesInApr
		,dt.TimesInMay
		,dt.TimesInJun
		,dt.TimesInJul
		,dt.TimesInAug
		,dt.TimesInSep
		,dt.TimesInOct
		,dt.TimesInNov
		,dt.TimesInDec
		,dt.TotalMinutes AS TotalCurrentYear
		,dt.TotalTimes AS TotalTimesCurrentYear
		,dt.TotalMinutesLeaveDays AS LeaveDeduction
		,pv.[Minutes] AS BalanceLastYear
		,ISNULL(pv.[Minutes], 0) AS BalanceLastYear
		,ISNULL(cu.[Minutes], 0) AS BalanceCurrentYear
		,ISNULL(cu.Overtime, 0) AS OvertimeBalance
		,pv.EmpId
		,e.NickName
		,e.FullName
		,al.AnnualLeaveBalance - (ISNULL(rq.TotalDays, 0)) AS AnnualLeaveBalance
		,g.Id AS TeamId
		,g.Code AS TeamCode
		,g.NAME AS TeamName
	FROM #PreviousYearBalance AS pv
		INNER JOIN #CurrentYearBalance AS cu ON pv.EmpId = cu.EmpId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON pv.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN #LateInEarlyOutSummary AS dt ON pv.EmpId = dt.EmpId
		LEFT JOIN #tblAnnualLeaveBalance AS al ON al.EmpId = pv.EmpId
		LEFT JOIN #PendingAnnualLeaveRequest AS rq ON e.Id = rq.EmpId
	WHERE e.GroupId NOT IN (11, 40, 1081)
	ORDER BY g.[Rank]
		,g.Code
		,e.Roles DESC
		,e.GroupDate;

	DROP TABLE #PreviousYearBalance;

	DROP TABLE #CurrentYearBalance;

	DROP TABLE #LateInEarlyOutSummary;

	DROP TABLE #PendingAnnualLeaveRequest;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_GetLateInEarlyOutDetail] --'2015-08-01', '2015-08-31'
    @FromDate DATETIME
    ,@ToDate DATETIME
    ,@GroupId INT = NULL
    ,@EmployeeId INT = NULL
    ,@IncludeLeaveDeduction BIT = 0
AS
/*
    Created: 20150805@Quinn.Vo
    Task: Get late in/ early out detail report
    DB: NexEPA
    Original: 

    Revisions:  
        - 20250507@Henry.Dinh: update group id from 11 to 1081
    Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT 
        la.WorkDate
        ,la.[Minutes]
        ,la.[Type] AS TypeId
        ,la.LeaveDeductionId
        ,la.ModifiedDate
        ,la.EmpId
        ,e.NickName
        ,e.FullName
        ,e.GroupId
        ,g.Code
        ,g.Name
    FROM dbo.LateInEarlyOutTrans AS la WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON la.EmpId = e.Id
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
    WHERE 
        e.GroupId NOT IN (1081,40)
        AND DATEDIFF(D, @FromDate, la.WorkDate) >= 0
        AND DATEDIFF(D, la.WorkDate, @ToDate) >= 0
        AND (@IncludeLeaveDeduction = 1 OR la.[Type] <> 0)
        AND (
            @EmployeeId IS NULL
            OR e.Id = @EmployeeId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_LateInEarlyOutSummary] @FromDate DATE
    ,@ToDate DATE
    ,@GroupId INT = NULL
    ,@EmpId INT = NULL
    ,@IsUserWarning BIT = NULL
AS
/*
    Created: 20160219@Tim.Trinh
    Task: Get late in/ early out summary report
    DB: NexHRM
    Revisions: 
        - 20250507@Henry.Dinh: update group id from 11 to 1081
    Original: 

*/
BEGIN
    SET NOCOUNT ON;

    IF OBJECT_ID('#LateInEarlyOutSummary') IS NOT NULL
    BEGIN
        DROP TABLE #LateInEarlyOutSummary;
    END;

    CREATE TABLE #LateInEarlyOutSummary (
        EmpId INT
        ,[Month] INT
        ,[Year] INT
        ,TotalTimes INT
        );

    INSERT INTO #LateInEarlyOutSummary
    SELECT DISTINCT tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
        ,COUNT(*)
    FROM dbo.LateInEarlyOutTrans AS tr WITH (NOLOCK)
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON tr.EmpId = e.Id
        AND e.GroupId NOT IN (1081, 40) -- management, expat
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
    WHERE tr.[Type] IN (1, 2)
        AND CAST(tr.WorkDate AS DATE) BETWEEN @FromDate
            AND @ToDate
        AND (
            @EmpId IS NULL
            OR e.Id = @EmpId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
    GROUP BY tr.EmpId
        ,MONTH(tr.WorkDate)
        ,YEAR(tr.WorkDate)
    HAVING COUNT(*) >= 4

    SELECT COUNT(tr.TotalTimes) AS TotalTimes
        ,e.Id
        ,e.NickName
        ,e.FullName
        ,e.GroupId
        ,g.Code
        ,g.[Name]
    FROM dbo.Employee AS e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
        LEFT JOIN #LateInEarlyOutSummary AS tr ON e.Id = tr.EmpId
    WHERE e.GroupId NOT IN (1081, 40) -- management, expat
        AND ISNULL(e.isDeleted, 0) = 0
        AND e.[Enabled] = 1
        AND (
            @EmpId IS NULL
            OR e.Id = @EmpId
            )
        AND (
            @GroupId IS NULL
            OR e.GroupId = @GroupId
            )
        AND (
            @IsUserWarning IS NULL
            OR @IsUserWarning = 0
            OR (
                @IsUserWarning = 1
                AND e.Id IN (
                    SELECT DISTINCT EmpId
                    FROM #LateInEarlyOutSummary
                    )
                )
            )
    GROUP BY e.Id
        ,e.NickName
        ,e.FullName
        ,e.Code
		,e.Roles
        ,e.GroupDate
        ,e.GroupId
        ,g.Code
        ,g.[Name]
        ,g.[Rank]
    ORDER BY g.[Rank]
        ,g.Code
        ,e.Roles DESC
        ,e.GroupDate

    SELECT EmpId
        ,CAST(CAST([Year] AS VARCHAR) + '-' + CAST([Month] AS VARCHAR) + '-' + CAST(1 AS VARCHAR) AS DATE) AS [Month]
        ,TotalTimes
    FROM #LateInEarlyOutSummary
    ORDER BY EmpId
        ,[Year]
        ,[Month]

    DROP TABLE #LateInEarlyOutSummary;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_LeaveSummary] @empid INT = NULL
    ,@groupid INT = NULL
    ,@year INT
AS
/*
Created: 2012/04/24@Dan
Task: leave summary report
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    DECLARE @fromdate DATETIME
        ,@todate DATETIME

    -- get the first date of year
    SET @fromdate = cast(@year AS VARCHAR(10)) + '/01/01'
    SET @todate = DATEADD(yy, 1, @fromdate)

    -- get the openning balace for annual leave to first date of year
    SELECT e.Id AS Empid
        ,SUM(ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0)) AS OpenBalance
    INTO #OpenBalance
    FROM Employee e WITH (NOLOCK)
        LEFT JOIN (
        SELECT *
        FROM dbo.LeaveTrans WITH (NOLOCK)
        WHERE ISNULL(Deleted, 0) = 0
            AND FromDate < @fromdate
        ) t ON e.Id = t.Empid
    --LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID=e.GroupId
    WHERE -- (FromDate < @fromdate OR FromDate IS NULL) 
        (
            @EmpId IS NULL
            OR e.id = @empid
            )
        -- AND ISNULL(t.Deleted,0)=0
        AND (
            @groupId IS NULL
            OR e.groupId = @groupId
            )
        AND (
            ISNULL(e.isDeleted, 0) = 0
            OR YEAR(ResignedDate) >= year(@todate)
            )
    GROUP BY e.Id

    -- select * from #OpenBalance order by Empid
    -- calculate the balance form the first date of year to @todate
    SELECT e.Id AS EmpId
        ,SUM(ISNULL(CreditDays, 0)) CreditDays
        ,SUM(ISNULL(DeductDays, 0)) DeductDays
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 1 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jan
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 2 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Feb
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 3 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Mar
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 4 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Apr
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 5 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) May
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 6 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jun
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 7 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Jul
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 8 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Aug
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 9 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Sep
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 10 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Oct
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 11 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) Nov
        ,SUM(CASE 
                WHEN MONTH(FromDate) = 12 THEN ISNULL(DeductDays, 0)
                ELSE 0
                END) DEC
    INTO #CurrenBalace
    FROM Employee e WITH (NOLOCK)
        LEFT JOIN (
        SELECT *
        FROM dbo.LeaveTrans WITH (NOLOCK)
        WHERE ISNULL(Deleted, 0) = 0
            AND FromDate >= @fromdate
            AND FromDate < @todate
        ) t
        -- FROM dbo.LeaveTrans t WITH (NOLOCK)    
        --    JOIN Employee e WITH (NOLOCK) 
        ON e.Id = t.Empid
    WHERE (
            @EmpId IS NULL
            OR e.id = @empid
            )
        -- AND ISNULL(t.Deleted,0)=0
        AND (
            @groupId IS NULL
            OR e.groupId = @groupId
            )
        AND (
            ISNULL(e.isDeleted, 0) = 0
            OR YEAR(ResignedDate) >= year(@todate)
            )
    GROUP BY e.ID

    --  select * from #CurrenBalace order by Empid
    -- show the report
    SELECT *
    FROM (
        SELECT cb.EmpId
            ,e.Code
            ,FullName
            ,NickName
			,GroupDate
			,Roles
            ,g.Id AS TeamId
            ,g.Code AS TeamCode
            ,g.NAME AS TeamName
            ,g.[Rank]
            ,ConfirmedDate
            ,ISNULL(OpenBalance, 0) [AnnualLeaveLastYear]
            ,ISNULL(CreditDays, 0) [AnnualLeaveCurrentYear]
            ,Jan
            ,Feb
            ,Mar
            ,Apr
            ,May
            ,Jun
            ,Jul
            ,Aug
            ,Sep
            ,Oct
            ,Nov
            ,DEC
            ,
            --        ISNULL(DeductDays,0) LeaveTaken,
            ISNULL(OpenBalance, 0) + ISNULL(CreditDays, 0) - ISNULL(DeductDays, 0) AS LeaveBalance
            ,'' AS Remark
        FROM #CurrenBalace cb
            LEFT JOIN #OpenBalance op ON OP.empid = cb.empid
            JOIN Employee e WITH (NOLOCK) ON e.Id = cb.Empid
            LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID = e.GroupId
        
        UNION ALL
        SELECT op.EmpId
            ,e.Code
            ,FullName
            ,NickName
			,Roles
			,GroupDate
            ,g.Id AS TeamId
            ,g.Code AS TeamCode
            ,g.NAME AS TeamName
            ,g.[Rank]
            ,ISNULL(OpenBalance, 0) [AnnualLeaveLastYear]
            ,0 AS [AnnualLeaveCurrentYear]
            ,0 AS Jan
            ,0 AS Feb
            ,0 AS Mar
            ,0 AS Apr
            ,0 AS May
            ,0 AS Jun
            ,0 AS Jul
            ,0 AS Aug
            ,0 AS Sep
            ,0 AS Oct
            ,0 AS Nov
            ,0 AS DEC
            ,ISNULL(OpenBalance, 0) AS LeaveBalance
            ,'' AS Remark
        FROM #OpenBalance op
            JOIN Employee e WITH (NOLOCK) ON e.Id = op.Empid
            LEFT JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.ID = e.GroupId
        WHERE Empid NOT IN (
                SELECT EmpID
                FROM #CurrenBalace cb
                )
        ) A
    ORDER BY a.[Rank]
        ,a.TeamCode
        ,a.Roles DESC
        ,a.GroupDate

    DROP TABLE #OpenBalance

    DROP TABLE #CurrenBalace
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_MonthlyAttendance]
	@fromtime datetime, 
	@totime datetime, 
	@empid int = null, 	
	@groupid int = null
AS

/*
Created: 2012/03/14@Dan
Task: monthly attendance report
DB: NexHR
Original: 

Revisions:    
Param's Explanation:	
*/

select 
	a.Id,
	EmpId,
	FullName,
	NickName,
	g.Name [Team],
	AttendanceDate,
	ShiftId,	
	s.code as ShiftCode,
	ExpectedInTime,
	ExpectedOutTime,
	InFixed,
	OutFixed,
	LateIn,
	EarlyOut,
	TotalWorkHours,
	Absence,
	LeaveTypeId,		
	Notes		
from dbo.EmployeeAttendance a WITH (NOLOCK) JOIN Employee e WITH (NOLOCK) 
		ON a.Empid= e.Id
		LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.Id=a.ShiftId			
		JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.groupid=g.Id
where (@empid is null or empid=@empid)		
	and (@groupid is null or g.ID=@groupid)
	and attendancedate between @fromtime and  @totime
order by NickName, attendancedate
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_RequestMonitor] @fromDate DATETIME
    ,@toDate DATETIME
    ,@requestType INT = 0
    ,@maxRequest INT
    ,@role INT
    ,@employeeID INT
AS
/*
Created: 2012/05/03@Dan
Task: show dashboard info for employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    DECLARE @CurrentDate DATETIME

    SET @CurrentDate = dbo.fnc_GetClientDateTime()

    IF OBJECT_ID('#Employee') IS NOT NULL
    BEGIN
        DROP TABLE #Employee
    END

    CREATE TABLE #Employee (EmpId INT)

    IF OBJECT_ID('#CheckInOutRequest') IS NOT NULL
    BEGIN
        DROP TABLE #CheckInOutRequest
    END

    CREATE TABLE #CheckInOutRequest (
        EmpId INT
        ,TotalRequest INT
        )

    IF OBJECT_ID('#LeaveRequestTimes') IS NOT NULL
    BEGIN
        DROP TABLE #LeaveRequestTimes
    END

    CREATE TABLE #LeaveRequestTimes (
        EmpId INT
        ,TotalRequest INT
        )

    IF OBJECT_ID('#LeaveRequestDays') IS NOT NULL
    BEGIN
        DROP TABLE #LeaveRequestDays
    END

    CREATE TABLE #LeaveRequestDays (
        EmpId INT
        ,TotalAnnualLeave NUMERIC(5, 1)
        ,TotalUnpaidLeave NUMERIC(5, 1)
        ,TotalOtherLeave NUMERIC(5, 1)
        )

    IF OBJECT_ID('#LateInEarlyOutAndAbsent') IS NOT NULL
    BEGIN
        DROP TABLE #LateInEarlyOutAndAbsent
    END

    CREATE TABLE #LateInEarlyOutAndAbsent (
        EmpId INT
        ,LateInEarlyOutTimes INT
        ,TotalAbsent INT
        ,TotalMissingCheckInOut INT
        )

    IF (
            @role = 3
            OR @role = 4
            )
    BEGIN
        INSERT INTO #Employee (EmpId)
        SELECT Id
        FROM dbo.Employee WITH (NOLOCK)
        WHERE ISNULL(isDeleted, 0) = 0
    END
    ELSE
    BEGIN
        INSERT INTO #Employee (EmpId)
		SELECT DISTINCT sup.EmpId
		FROM dbo.EmployeeSupDetail AS sup WITH (NOLOCK)
		WHERE sup.ApproverId = @employeeID
    END

    INSERT INTO #CheckInOutRequest (
        EmpId
        ,TotalRequest
        )
    SELECT wk.EmpId
        ,COUNT(1)
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN #Employee AS e ON wk.EmpId = e.EmpId
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON wk.EmpId = cl.EmpId
        AND DATEDIFF(D, wk.FromDate, cl.WorkDate) = 0
    WHERE wk.RequestType IN (1, 2)
        AND wk.[Status] IN (1, 2)
        AND DATEDIFF(D, @fromdate, wk.FromDate) > = 0
        AND DATEDIFF(D, wk.FromDate, @toDate) > = 0
        AND cl.IsHoliday <> 1
		AND cl.IsOffDay <> 1 -- l? or ngày cu?i tu?n
    GROUP BY wk.EmpId

    INSERT INTO #LeaveRequestTimes (
        EmpId
        ,TotalRequest
        )
    SELECT wk.EmpId
        ,COUNT(1)
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN #Employee AS e ON wk.EmpId = e.EmpId
    WHERE wk.RequestType = 6
        AND wk.[Status] IN (1, 2)
        AND (
            (
                DATEDIFF(D, @fromdate, wk.FromDate) > = 0
                AND DATEDIFF(D, wk.FromDate, @toDate) > = 0
                )
            OR (
                DATEDIFF(D, @fromdate, wk.ToDate) > = 0
                AND DATEDIFF(D, wk.ToDate, @toDate) > = 0
                )
            )
    GROUP BY wk.EmpId

    INSERT INTO #LeaveRequestDays (
        EmpId
        ,TotalAnnualLeave
        ,TotalUnpaidLeave
        ,TotalOtherLeave
        )
    SELECT wk.EmpId
        ,ISNULL(SUM(CASE 
                    WHEN (wk.LeaveTypeID IN (2, 30)) THEN (CASE 
                            WHEN wk.TotalDays = 0.5 THEN 0.5
                            ELSE 1 END
                            )
                    ELSE 0
                    END), 0)
        ,ISNULL(SUM(CASE 
                    WHEN (wk.LeaveTypeID = 26) THEN (CASE 
                            WHEN wk.TotalDays = 0.5 THEN 0.5
                            ELSE 1 END
                            )
                    ELSE 0
                    END), 0)
        ,ISNULL(SUM(CASE 
                    WHEN (wk.LeaveTypeID NOT IN (2, 26, 30)) THEN (CASE 
                            WHEN wk.TotalDays = 0.5 THEN 0.5
                            ELSE 1 END
                            )
                    ELSE 0
                    END), 0)
    FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN #Employee AS e ON wk.EmpId = e.EmpId
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON DATEDIFF(D, wk.FromDate, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, wk.ToDate) >= 0
        AND wk.EmpId = cl.EmpId
        AND cl.IsOffDay <> 1
        AND cl.IsHoliday <> 1
    WHERE wk.RequestType = 6
        AND wk.[Status] IN (1, 2)
        AND DATEDIFF(D, @fromDate, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, @toDate) >= 0
    GROUP BY wk.EmpId

    INSERT INTO #LateInEarlyOutAndAbsent (
        EmpId
        ,LateInEarlyOutTimes
        ,TotalAbsent
        ,TotalMissingCheckInOut
        )
    SELECT att.EmpId
        ,SUM((CASE 
                WHEN att.LateIn > 0 THEN 1
                ELSE 0 END
                ) + (CASE 
                WHEN att.EarlyOut > 0 THEN 1
                ELSE 0 END
                ))
        ,SUM(CASE 
                WHEN (
                        ISNULL(att.Absence, 0) = 1
                        AND cl.IsOffDay = 0
                        AND cl.IsHoliday = 0
                        AND ISNULL(att.LeaveDay, 0) <> 1
                        AND DATEDIFF(D, (att.AttendanceDate + ' ' + s.EndCheckInTime), @CurrentDate) > 0
                        ) THEN 1
                ELSE 0
                END)
        ,SUM((CASE 
                WHEN (
                        ISNULL(Absence, 0) = 0
                        AND att.InFixed IS NULL
                        AND DATEDIFF(D, (att.AttendanceDate + ' ' + s.EndCheckInTime), @CurrentDate) > 0
                        ) THEN 1
                ELSE 0 END
                ) + (CASE 
                WHEN (
                        ISNULL(Absence, 0) = 0
                        AND att.OutFixed IS NULL
                        AND DATEDIFF(D, (att.AttendanceDate + ' ' + s.EndCheckInTime), @CurrentDate) > 0
                        ) THEN 1
                ELSE 0 END
                ))
    FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
        INNER JOIN #Employee AS e ON att.EmpId = e.EmpId
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON cl.EmpId = att.EmpId
        AND cl.Workdate = att.AttendanceDate
        LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.Id = att.ShiftId
    WHERE DATEDIFF(D, @fromDate, cl.WorkDate) >= 0
        AND DATEDIFF(D, cl.WorkDate, @toDate) >= 0
    GROUP BY att.EmpId

    SELECT ISNULL(chk.TotalRequest, 0) AS TotalCheckInOutTimes
        ,ISNULL(lti.TotalRequest, 0) AS TotalLeaveTimes
        ,ISNULL(lda.TotalAnnualLeave, 0) AS TotalAnnualLeave
        ,ISNULL(lda.TotalUnpaidLeave, 0) AS TotalUnpaidLeave
        ,ISNULL(lda.TotalOtherLeave, 0) AS TotalOtherLeave
        ,ISNULL(li.LateInEarlyOutTimes, 0) AS LateInEarlyOutTimes
        ,ISNULL(li.TotalAbsent, 0) AS TotalAbsentTimes
        ,ISNULL(li.TotalMissingCheckInOut, 0) AS TotalMissingCheckInOutTimes
        ,ee.EmpId
        ,e.FullName
        ,e.NickName
        ,g.Id AS GroupId
        ,g.Code
        ,g.NAME
    FROM #Employee AS ee
        INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.ID = ee.EmpId
        INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON g.Id = e.GroupID
        LEFT JOIN #CheckInOutRequest AS chk ON e.Id = chk.EmpId
        LEFT JOIN #LeaveRequestTimes AS lti ON e.Id = lti.EmpId
        LEFT JOIN #LeaveRequestDays AS lda ON e.Id = lda.EmpId
        LEFT JOIN #LateInEarlyOutAndAbsent AS li ON e.Id = li.EmpId
    WHERE 
		ISNULL(e.IsSpecialUser, 0) = 0
		AND
		(
			( (@requestType = 1) AND (			lti.TotalRequest >= @maxRequest
            OR lda.TotalAnnualLeave >= @maxRequest
            OR lda.TotalUnpaidLeave >= @maxRequest
            OR lda.TotalOtherLeave >= @maxRequest ) )
            
            OR ( (@requestType = 2) AND (       chk.TotalRequest >= @maxRequest
            OR li.TotalMissingCheckInOut >= @maxRequest))
            
            OR ( (@requestType = 3) AND (li.LateInEarlyOutTimes >= @maxRequest))
            
            OR ( (@requestType = 4) AND (li.TotalAbsent >= @maxRequest))
            
            OR ( (@requestType = 0) AND (                         
				lti.TotalRequest >= @maxRequest
				OR lda.TotalAnnualLeave >= @maxRequest
				OR lda.TotalUnpaidLeave >= @maxRequest
				OR lda.TotalOtherLeave >= @maxRequest   
				OR chk.TotalRequest >= @maxRequest
				OR li.TotalMissingCheckInOut >= @maxRequest            
				OR li.LateInEarlyOutTimes >= @maxRequest            
				OR li.TotalAbsent >= @maxRequest
				))                              
            )
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_WorkingHoursStatistic]
	 @fromdate DATETIME
	,@todate DATETIME
	,@FromDateLastMonth DATETIME
	,@ToDateLastMonth DATETIME
	,@empid INT = NULL
	,@groupid INT = NULL
	,@departmentid INT = NULL
AS
/*
Created: 2012/04/23@Dan
Task: Working Hours Summary REPORT
DB: NexHR
Original: 

Revisions:   
  20170411@Raymond.Le: create SP Report_WorkingHoursStatistic
  20191206@Merlin.Le: PreMonthSickLeave contains Maternity and Parternity
  20250515@Henry.Dinh: Add DepartmentId and DepartmentName
Param's Explanation:

*/

/* Script run
USE [NexHRM]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[Report_WorkingHoursStatistic]
		@fromdate = N'8/1/2016 12:00:00 AM',
		@todate = N'8/25/2016 12:00:00 AM',
		@FromDateLastMonth = N'7/26/2016 12:00:00 AM',
		@ToDateLastMonth = N'7/31/2016 12:00:00 AM'

SELECT	'Return Value' = @return_value

GO
*/

/* Result
PreMonthSickLeave of Julien is 2
*/

DECLARE @UnpaidLeaveId INT
	,@SickLeave INT
	,@BusinessTrip INT
	,@MaternityLeave INT
	,@PaternityLeave INT
	,@BusinessLeave INT


SELECT @UnpaidLeaveId = 26
	,@SickLeave = 28
	,@BusinessTrip = 9
	,@MaternityLeave = 13
	,@PaternityLeave = 27
	,@BusinessLeave = 32

CREATE TABLE #tmpWorkingHoursStatistic(
	TotalWorkingDay INT
	,ActualWorkingDay FLOAT
	,AnnualLeave FLOAT
	,PaidLeave FLOAT
	,SickLeave FLOAT
	,InsuranceLeave FLOAT
	,UnpaidLeave FLOAT
	,BusinessLeave FLOAT
	,WorkFromHome FLOAT
	,EmpId INT
	,Code VARCHAR(20)
	,NickName NVARCHAR(20)
	,FullName NVARCHAR(50)
	,GroupId INT
	,GroupCode VARCHAR(20)
	,GroupName NVARCHAR(50)
	,DepartmentId INT
	,DepartmentName NVARCHAR(50)
)

CREATE TABLE #tmpPreMonthUPL (
	EmpId INT
	,PreMonthTotalWorkingDay INT
	,PreMonthSickLeave FLOAT 
	,PreMonthUnpaidLeave FLOAT
	,PreMonthBusinessLeave FLOAT
	,PreWorkFromHome FLOAT
	,PreInsuranceLeave FLOAT
	,PrePaidLeave FLOAT
	,PreAnnualLeave FLOAT
)

INSERT INTO #tmpPreMonthUPL(
	EmpId
	,PreMonthTotalWorkingDay 
	,PreMonthSickLeave  
	,PreMonthUnpaidLeave 
	,PreMonthBusinessLeave 
	,PreWorkFromHome
	,PreInsuranceLeave
	,PrePaidLeave
	,PreAnnualLeave
)
SELECT pm.EmpId
	,pm.TotalWorkingDay 
	,pm.SickLeave 
	,pm.UnpaidLeave
	,BusinessLeave
	,WorkFromHome
	,InsuranceLeave
	,PaidLeave
	,AnnualLeave
FROM (
	SELECT c.EmpId
		,SUM(CASE 
				WHEN ISNULL(c.IsOffDay, 0) = 0
					AND ISNULL(c.IsHoliday, 0) = 0 
					THEN 1
				ELSE 0
				END) AS TotalWorkingDay
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (c.LeaveTypeId = @SickLeave
					OR c.LeaveTypeId2 = @SickLeave
					OR c.LeaveTypeId = @MaternityLeave
					OR c.LeaveTypeId2 = @MaternityLeave
					OR c.LeaveTypeId = @PaternityLeave
					OR c.LeaveTypeId2 = @PaternityLeave) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS SickLeave
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @UnpaidLeaveId
					OR c.LeaveTypeId2 = @UnpaidLeaveId THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [UnpaidLeave]
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @BusinessLeave
					OR c.LeaveTypeId2 = @BusinessLeave THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS BusinessLeave
		,SUM(CASE WHEN IsWorkFromHome = 1
			AND c.IsOffDay = 0
			AND c.IsHoliday = 0
		THEN (1 - ISNULL(c.LeaveDay, 0) +
		CASE WHEN ISNULL(c.LeaveDay, 0) = 1 THEN
			CASE WHEN ISNULL(c.LeaveTypeId2, 0) = 0
			THEN 
				CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 1 ELSE 0 END
			ELSE
				CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
					AND ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
				THEN 1 
				ELSE
					CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
						OR ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
					THEN 0.5
					ELSE 0
					END
				END
			END
		ELSE
			CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 0.5 ELSE 0 END
		END) 
		ELSE 0 END)
		AS WorkFromHome
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsInsuranceLeave = 1
							AND lv1.id <> @SickLeave
							)
						OR (
							lv2.IsInsuranceLeave = 1
							AND lv2.id <> @SickLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.IsInsuranceLeave = 0
								OR lv1.Id = @SickLeave
								OR lv2.IsInsuranceLeave = 0
								OR lv2.Id = @SickLeave
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS InsuranceLeave
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsSalaryDeduction = 0
							AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
							AND lv1.IsALDeduction = 0
							AND lv1.Id <> @BusinessTrip
							AND lv1.Id <> @BusinessLeave
							)
						OR (
							lv2.IsSalaryDeduction = 0
							AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
							AND lv2.IsALDeduction = 0
							AND lv2.Id <> @BusinessTrip
							AND lv2.Id <> @BusinessLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								NOT (
									lv1.IsSalaryDeduction = 0
									AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
									AND lv1.IsALDeduction = 0
									AND lv1.Id <> @BusinessTrip
									AND lv1.Id <> @BusinessLeave
									)
								OR NOT (
									lv2.IsSalaryDeduction = 0
									AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
									AND lv2.IsALDeduction = 0
									AND lv2.Id <> @BusinessTrip
									AND lv2.Id <> @BusinessLeave
									)
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [PaidLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.IsALDeduction = 1
						OR lv2.IsALDeduction = 1
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS AnnualLeave
	FROM dbo.EmployeeCalendar c WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance a WITH (NOLOCK) ON c.empid = a.empid
		AND c.workdate = a.AttendanceDate
		INNER JOIN employee e WITH (NOLOCK) ON e.id = c.empid
		INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.Id = e.GroupId
		LEFT JOIN dbo.LeaveType lv1 WITH (NOLOCK) ON c.LeaveTypeId = lv1.Id
		LEFT JOIN dbo.LeaveType lv2 WITH (NOLOCK) ON c.LeaveTypeId2 = lv2.Id
	WHERE e.IsSpecialUser = 0
		AND (
			@empid IS NULL
			OR c.empid = @empid
			)
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND WorkDate BETWEEN @FromDateLastMonth
			AND @ToDateLastMonth
		AND (
			@groupid IS NULL
			OR e.GroupId = @groupid
			)
	GROUP BY c.empid
	) AS pm

SELECT
	--(ActualWorkingDay + PaidLeave + UnpaidLeave + SickLeave + AccidentLaborLeave) AS WorkingDay
	(TotalWorkingDay + ISNULL(Holiday, 0)) AS TotalWorkingDay
	,(ActualWorkingDay + ISNULL(BusinessTrip, 0) + ISNULL(BusinessLeave, 0) + ISNULL(Holiday, 0)) AS ActualWorkingDay
	,AnnualLeave
	,PaidLeave
	,SickLeave
	,InsuranceLeave
	,UnpaidLeave
	,BusinessLeave
	,WorkFromHome
	,tpu.PreMonthTotalWorkingDay
	,tpu.PreMonthSickLeave
	,tpu.PreMonthUnpaidLeave
	,tpu.PreMonthBusinessLeave
	,tpu.PreWorkFromHome
	,tpu.PreInsuranceLeave
	,tpu.PrePaidLeave
	,tpu.PreAnnualLeave
	,A.EmpId
	,A.Code
	,NickName
	,FullName
	,A.DepartmentId
	,d.Name AS DepartmentName
	,GroupId
	,GroupCode
	,GroupName
FROM (
	SELECT c.empid
		,e.Code
		,FullName
		,NickName
		,e.Roles
		,e.GroupDate
		,g.Id AS GroupId
		,g.Code AS GroupCode
		,g.NAME AS GroupName
		,g.Rank
		,g.DepartmentId
		,SUM(CASE 
				WHEN ISNULL(c.IsOffDay, 0) = 0
					AND ISNULL(c.IsHoliday, 0) = 0 
					THEN 1
				ELSE 0
				END) AS TotalWorkingDay
		,SUM(CASE 
				WHEN Absence IS NOT NULL
					AND Absence = 0
					AND IsLeaveDay = 0 THEN 1
				WHEN Absence IS NOT NULL
					AND Absence = 0
					AND IsLeaveDay = 1
					AND c.LeaveDay < 1 THEN 0.5
				ELSE 0
				END) [ActualWorkingDay]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.IsALDeduction = 1
						OR lv2.IsALDeduction = 1
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS AnnualLeave
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsSalaryDeduction = 0
							AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
							AND lv1.IsALDeduction = 0
							AND lv1.Id <> @BusinessTrip
							AND lv1.Id <> @BusinessLeave
							)
						OR (
							lv2.IsSalaryDeduction = 0
							AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
							AND lv2.IsALDeduction = 0
							AND lv2.Id <> @BusinessTrip
							AND lv2.Id <> @BusinessLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								NOT (
									lv1.IsSalaryDeduction = 0
									AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
									AND lv1.IsALDeduction = 0
									AND lv1.Id <> @BusinessTrip
									AND lv1.Id <> @BusinessLeave
									)
								OR NOT (
									lv2.IsSalaryDeduction = 0
									AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
									AND lv2.IsALDeduction = 0
									AND lv2.Id <> @BusinessTrip
									AND lv2.Id <> @BusinessLeave
									)
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [PaidLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.Id = @BusinessTrip
						OR lv2.Id = @BusinessTrip
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.Id <> @BusinessTrip
								OR lv2.Id <> @BusinessTrip
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [BusinessTrip]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.Id = @BusinessLeave
						OR lv2.Id = @BusinessLeave
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.Id <> @BusinessLeave
								OR lv2.Id <> @BusinessLeave
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS BusinessLeave
		,SUM(CASE WHEN IsWorkFromHome = 1
			AND c.IsOffDay = 0
			AND c.IsHoliday = 0
		THEN (1 - ISNULL(c.LeaveDay, 0) +
		CASE WHEN ISNULL(c.LeaveDay, 0) = 1 THEN
			CASE WHEN ISNULL(c.LeaveTypeId2, 0) = 0
			THEN 
				CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 1 ELSE 0 END
			ELSE
				CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
					AND ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
				THEN 1 
				ELSE
					CASE WHEN ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
						OR ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave)
					THEN 0.5
					ELSE 0
					END
				END
			END
		ELSE
			CASE WHEN ISNULL(c.LeaveTypeId, 0) IN (@BusinessTrip, @BusinessLeave) THEN 0.5 ELSE 0 END
		END) 
		ELSE 0 END)
		AS WorkFromHome
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @SickLeave
					OR c.LeaveTypeId2 = @SickLeave THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [SickLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsInsuranceLeave = 1
							AND lv1.id <> @SickLeave
							)
						OR (
							lv2.IsInsuranceLeave = 1
							AND lv2.id <> @SickLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.IsInsuranceLeave = 0
								OR lv1.Id = @SickLeave
								OR lv2.IsInsuranceLeave = 0
								OR lv2.Id = @SickLeave
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS InsuranceLeave
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @UnpaidLeaveId
					OR c.LeaveTypeId2 = @UnpaidLeaveId THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [UnpaidLeave]
		
		,SUM(CASE WHEN c.IsHoliday = 1 THEN 1 ELSE 0 END) Holiday
	FROM dbo.EmployeeCalendar c WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance a WITH (NOLOCK) ON c.empid = a.empid
		AND c.workdate = a.AttendanceDate
		INNER JOIN employee e WITH (NOLOCK) ON e.id = c.empid
		INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.Id = e.GroupId
		LEFT JOIN dbo.LeaveType lv1 WITH (NOLOCK) ON c.LeaveTypeId = lv1.Id
		LEFT JOIN dbo.LeaveType lv2 WITH (NOLOCK) ON c.LeaveTypeId2 = lv2.Id
	WHERE e.IsSpecialUser = 0
		AND (
			@empid IS NULL
			OR c.empid = @empid
			)
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND WorkDate BETWEEN @fromdate
			AND @todate
		AND (
			@groupid IS NULL
			OR GroupId = @groupid
			)
		AND (
			@departmentid IS NULL
			OR g.DepartmentId = @departmentid
			)
	GROUP BY c.empid
		,e.Code
		,FullName
		,NickName
		,e.Roles
		,e.GroupDate
		,g.Id
		,g.Code
		,g.NAME
		,[Rank]
		,e.Id
		,g.DepartmentId
	) A
	LEFT JOIN #tmpPreMonthUPL AS tpu
	ON tpu.EmpId = A.EmpId
	LEFT JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = A.DepartmentId
ORDER BY [Rank]
	,a.GroupCode
	,a.Roles DESC
	,a.GroupDate


DROP TABLE #tmpPreMonthUPL
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_WorkingHoursSummary] @fromdate DATETIME
	,@todate DATETIME
	,@empid INT = NULL
	,@groupid INT = NULL
    ,@departmentid INT = NULL
AS
/*
Created: 2012/04/23@Dan
Task: Working Hours Summary REPORT
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
DECLARE @UnpaidLeaveId INT
	,@SickLeave INT
	,@BusinessTrip INT
	,@BusinessLeave INT

SELECT @UnpaidLeaveId = 26
	,@SickLeave = 28
	,@BusinessTrip = 9
	,@BusinessLeave = 32

SELECT
	--(ActualWorkingDay + PaidLeave + UnpaidLeave + SickLeave + AccidentLaborLeave) AS WorkingDay
	WorkingDay
	,(ActualWorkingDay + ISNULL(BusinessTrip, 0) + ISNULL(BusinessLeave, 0)) AS ActualWorkingDay
	,(AnnualLeave + ISNULL(upl.TotalAnnualLeave, 0)) AS AnnualLeave
	,PaidLeave
	,SickLeave
	,InsuranceLeave
	,(UnpaidLeave + ISNULL(upl.TotalUnpaidLeave, 0)) AS UnpaidLeave
	,LateIn
	,EarlyOut
	,TotalHours
	,AverageHours
	,A.EmpId
	,Code
	,NickName
	,FullName
	,DepartmentName
	,GroupId
	,GroupCode
	,GroupName
FROM (
	SELECT c.empid
		,e.Code
		,FullName
		,NickName
		,Roles
		,GroupDate		
		,d.Name AS DepartmentName
		,g.Id AS GroupId
		,g.Code AS GroupCode
		,g.NAME AS GroupName
		,g.Rank
		,SUM(CASE 
				WHEN ISNULL(c.IsOffDay, 0) = 0
					AND ISNULL(c.IsHoliday, 0) = 0 THEN 1
				ELSE 0
				END) AS WorkingDay
		,SUM(CASE 
				WHEN Absence IS NOT NULL
					AND Absence = 0
					AND IsLeaveDay = 0 THEN 1
				WHEN Absence IS NOT NULL
					AND Absence = 0
					AND IsLeaveDay = 1
					AND c.LeaveDay < 1 THEN 0.5
				ELSE 0
				END) [ActualWorkingDay]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.IsALDeduction = 1
						OR lv2.IsALDeduction = 1
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS AnnualLeave
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsSalaryDeduction = 0
							AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
							AND lv1.IsALDeduction = 0
							AND lv1.Id <> @BusinessTrip
							AND lv1.Id <> @BusinessLeave
							)
						OR (
							lv2.IsSalaryDeduction = 0
							AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
							AND lv2.IsALDeduction = 0
							AND lv2.Id <> @BusinessTrip
							AND lv2.Id <> @BusinessLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								NOT (
									lv1.IsSalaryDeduction = 0
									AND ISNULL(lv1.IsInsuranceLeave, 0) = 0
									AND lv1.IsALDeduction = 0
									AND lv1.Id <> @BusinessTrip
									AND lv1.Id <> @BusinessLeave
									)
								OR NOT (
									lv2.IsSalaryDeduction = 0
									AND ISNULL(lv2.IsInsuranceLeave, 0) = 0
									AND lv2.IsALDeduction = 0
									AND lv2.Id <> @BusinessTrip
									AND lv2.Id <> @BusinessLeave
									)
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [PaidLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						lv1.Id = @BusinessTrip
						OR lv2.Id = @BusinessTrip
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.Id <> @BusinessTrip
								OR lv2.Id <> @BusinessTrip
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS [BusinessTrip]
		,SUM(CASE 
			WHEN isOffday = 0
				AND c.isHoliday = 0
				AND IsLeaveDay = 1
				AND (
					lv1.Id = @BusinessLeave
					OR lv2.Id = @BusinessLeave
					) THEN CASE 
					WHEN ISNULL(lv2.Id, 0) <> 0
						AND (
							lv1.Id <> @BusinessLeave
							OR lv2.Id <> @BusinessLeave
							) THEN 0.5
					ELSE c.LeaveDay
					END
			ELSE 0
			END) AS [BusinessLeave]
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @SickLeave
					OR c.LeaveTypeId2 = @SickLeave THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [SickLeave]
		,SUM(CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND (
						(
							lv1.IsInsuranceLeave = 1
							AND lv1.id <> @SickLeave
							)
						OR (
							lv2.IsInsuranceLeave = 1
							AND lv2.id <> @SickLeave
							)
						) THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND (
								lv1.IsInsuranceLeave = 0
								OR lv1.Id = @SickLeave
								OR lv2.IsInsuranceLeave = 0
								OR lv2.Id = @SickLeave
								) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0
				END) AS InsuranceLeave
		,SUM((CASE 
				WHEN isOffday = 0
					AND c.isHoliday = 0
					AND IsLeaveDay = 1
					AND c.LeaveTypeId = @UnpaidLeaveId
					OR c.LeaveTypeId2 = @UnpaidLeaveId THEN CASE 
						WHEN ISNULL(lv2.Id, 0) <> 0
							AND ISNULL(lv1.Id, 0) <> ISNULL(lv2.Id, 0) THEN 0.5
						ELSE c.LeaveDay
						END
				ELSE 0 END
				)) AS [UnpaidLeave]
		,SUM(a.LateIn) LateIn
		,SUM(a.EarlyOut) EarlyOut
		,SUM(ISNULL(a.TotalWorkHours, 0)) TotalHours
		,SUM(ISNULL(a.TotalWorkHours, 0)) / CASE 
			WHEN SUM(CASE 
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 0 THEN 1
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 1
							AND c.LeaveDay < 1
							AND c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave) THEN 0.5
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
							AND (
								c.LeaveTypeId2 IS NULL
								OR c.LeaveTypeId2 IN (@BusinessTrip, @BusinessLeave)
								) THEN 1
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND (
								c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
								)
							AND (
								c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) NOT IN (@BusinessTrip, @BusinessLeave)
								) THEN 0.5
						ELSE 0
						END) = 0 THEN 1
			ELSE SUM(CASE 
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 0 THEN 1
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 1
							AND c.LeaveDay < 1
							AND c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave) THEN 0.5
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
							AND (
								c.LeaveTypeId2 IS NULL
								OR c.LeaveTypeId2 IN (@BusinessTrip, @BusinessLeave)
								) THEN 1
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND (
								c.LeaveTypeId IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) IN (@BusinessTrip, @BusinessLeave)
								)
							AND (
								c.LeaveTypeId NOT IN (@BusinessTrip, @BusinessLeave)
								OR ISNULL(c.LeaveTypeId2, 0) NOT IN (@BusinessTrip, @BusinessLeave)
								) THEN 0.5
						ELSE 0
						END)
			END AS AverageHours
	FROM dbo.EmployeeCalendar c WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance a WITH (NOLOCK) ON c.empid = a.empid
		AND c.workdate = a.AttendanceDate
		INNER JOIN employee e WITH (NOLOCK) ON e.id = c.empid
		INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON g.Id = e.GroupId
		LEFT JOIN dbo.Department d WITH (NOLOCK) ON d.DepartmentId = g.DepartmentId
		LEFT JOIN dbo.LeaveType lv1 WITH (NOLOCK) ON c.LeaveTypeId = lv1.Id
		LEFT JOIN dbo.LeaveType lv2 WITH (NOLOCK) ON c.LeaveTypeId2 = lv2.Id
	WHERE e.IsSpecialUser = 0
		AND (
			@empid IS NULL
			OR c.empid = @empid
			)
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND WorkDate BETWEEN @fromdate
			AND @todate
		AND (
			@departmentid IS NULL
			OR g.DepartmentId = @departmentid
			)
		AND (
			@groupid IS NULL
			OR GroupId = @groupid
			)
	GROUP BY c.empid
		,e.Code
		,FullName
		,NickName
		,Roles
		,GroupDate
		,g.Id
		,g.Code
		,g.NAME
		,[Rank]
		,e.Id
		,d.Name
	) A
	LEFT JOIN (
	SELECT EmpId
		,SUM(CASE 
				WHEN IsAnnual = 0 THEN ISNULL(LeaveDays, 0)
				ELSE 0
				END) AS TotalUnpaidLeave
		,SUM(CASE 
				WHEN IsAnnual = 1 THEN ISNULL(LeaveDays, 0)
				ELSE 0
				END) AS TotalAnnualLeave
	FROM dbo.LeaveConversion AS lv WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON lv.EmpId = e.Id
	WHERE lv.[Type] = 1
		AND (
			@empid IS NULL
			OR empid = @empid
			)
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND [Date] BETWEEN @fromdate
			AND @todate
		AND (
			@groupid IS NULL
			OR GroupId = @groupid
			)
	GROUP BY EmpId
	) AS upl ON A.EmpId = upl.EmpId
ORDER BY [Rank]
	,a.GroupCode
	,a.Roles DESC
	,a.GroupDate
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Delete]
	@ScheduledMessageId INT
	 ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: delete message by Id
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	DELETE
	FROM dbo.ScheduledMessage
	WHERE ScheduledMessageId = @ScheduledMessageId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Get]
	@YEAR INT
	,@CreatedBy INT
AS
/*
Created: 20190325@Elton.ton
Task: Get messages by year
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT sm.ScheduledMessageId
		,sm.ConversationId
		,c.ConversationName
		,sm.SendDate
		,sm.Content
		,sm.CreatedBy
		,e.NickName
		,sm.IsSent
	FROM dbo.ScheduledMessage sm WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON sm.CreatedBy = e.Id
		INNER JOIN dbo.Conversation c WITH (NOLOCK) ON sm.ConversationId = c.ConversationId
	WHERE YEAR(sm.SendDate) = @YEAR AND sm.CreatedBy = @CreatedBy
	ORDER BY sm.SendDate DESC
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_GetById]
	@ScheduledMessageId INT
AS
/*
Created: 20190325@Elton.ton
Task: Get message by Id
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT sm.ScheduledMessageId
		,sm.ConversationId
		,c.ConversationName
		,sm.SendDate
		,sm.Content
		,sm.CreatedBy
		,e.NickName
		,sm.IsSent
		,sm.StepNumber
		,sm.RepeatTypeID
	FROM dbo.ScheduledMessage sm WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON sm.CreatedBy = e.Id
		INNER JOIN dbo.Conversation c WITH (NOLOCK) ON sm.ConversationId = c.ConversationId
	WHERE sm.ScheduledMessageId = @ScheduledMessageId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_GetMessagesToProcess]
    @FromDate DATETIME
    ,@ToDate DATETIME
AS
/*
Created: 20190325@Elton.ton
Task: Get message by SendDate
DB: NexHRM
Original:
    - 20250507@Henry: Update hardcode conversationId
Revisions:
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    SELECT sm.ScheduledMessageId
        ,sm.ConversationId
        ,sm.Content
        ,sm.CronExpression
        ,c.SkypeConversationId
    FROM dbo.ScheduledMessage sm WITH (NOLOCK)
        INNER JOIN dbo.Conversation c WITH (NOLOCK) ON sm.ConversationId = c.ConversationId
    WHERE ((sm.SendDate BETWEEN @FromDate AND @ToDate
            AND sm.CronExpression IS NULL)
            OR (@ToDate >= sm.SendDate
            AND sm.CronExpression IS NOT NULL))
        AND sm.IsSent = 0
        AND CHARINDEX('B08J3UJ8WMA:' , c.SkypeConversationId ) = 1
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Insert]
	@ConversationId INT
	,@SendDate DATETIME
	,@Content NVARCHAR(MAX)
	,@CreatedBy INT
	,@CronExpression VARCHAR(50) = null
	,@StepNumber INT = null
	,@RepeatTypeId INT = null
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: 
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	IF (@RepeatTypeId IS NULL)
		INSERT INTO [dbo].[ScheduledMessage]
           ([ConversationId]
           ,[SendDate]
           ,[Content]
           ,[CreatedDate]
           ,[CreatedBy])
		VALUES
           (@ConversationId
           ,@SendDate
           ,@Content
           ,dbo.fnc_GetClientDateTime()
           ,@CreatedBy)
	ELSE
		INSERT INTO [dbo].[ScheduledMessage]
           ([ConversationId]
           ,[SendDate]
           ,[Content]
           ,[CreatedDate]
           ,[CreatedBy]
		   ,[CronExpression]
		   ,[StepNumber]
		   ,[RepeatTypeID])
		VALUES
           (@ConversationId
           ,@SendDate
           ,@Content
           ,dbo.fnc_GetClientDateTime()
           ,@CreatedBy
		   ,@CronExpression
		   ,@StepNumber
		   ,@RepeatTypeId)
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_Update]
	@ScheduledMessageId INT
	,@ConversationId INT
	,@SendDate DATETIME
	,@Content NVARCHAR(MAX)
	,@CronExpression VARCHAR(50) = null
	,@StepNumber INT = null
	,@RepeatTypeId INT = null
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: update message
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;
	IF (@RepeatTypeId IS NULL)
		UPDATE [dbo].[ScheduledMessage] WITH (ROWLOCK, UPDLOCK)
		SET [ConversationId] = @ConversationId
		  ,[SendDate] = @SendDate
		  ,[Content] = @Content
		  ,[CronExpression] = null
		  ,[StepNumber] = null
		  ,[RepeatTypeId] = null
		WHERE ScheduledMessageId = @ScheduledMessageId
	ELSE
		UPDATE [dbo].[ScheduledMessage] WITH (ROWLOCK, UPDLOCK)
		SET [ConversationId] = @ConversationId
		  ,[SendDate] = @SendDate
		  ,[Content] = @Content
		  ,[CronExpression] = @CronExpression
		  ,[StepNumber] = @StepNumber
		  ,[RepeatTypeId] = @RepeatTypeId
		WHERE ScheduledMessageId = @ScheduledMessageId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledMessage_UpdateStatus]
	@ScheduledMessageIds VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190325@Elton.ton
Task: update selected messages are sent
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	UPDATE [dbo].[ScheduledMessage] WITH (ROWLOCK, UPDLOCK)
	SET IsSent = 1
	WHERE EXISTS (
		SELECT 1
		FROM dbo.fnc_strSplit(@ScheduledMessageIds, ',')
		WHERE val = ScheduledMessageId
	)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ScheduledVegetarianMessage_Get]
	@YEAR INT
AS
/*
Created: 20233103@Brian.nguyen
Task: Get Vegetarian messages by year
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

	SELECT sm.ScheduledMessageId
		,sm.ConversationId
		,c.ConversationName
		,sm.SendDate
		,sm.Content
		,sm.CreatedBy
		,e.NickName
		,sm.IsSent
	FROM dbo.ScheduledMessage sm WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON sm.CreatedBy = e.Id
		INNER JOIN dbo.Conversation c WITH (NOLOCK) ON sm.ConversationId = c.ConversationId
	WHERE YEAR(sm.SendDate) = @YEAR AND sm.Content LIKE N'%Ð?t com chay%'
	ORDER BY sm.SendDate DESC
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Shift_Get] 
AS
/*
Created: 20141002@Quinn.Vo
Task: Get all shifts
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT Id AS ShiftId
    ,Code
    ,Name
    ,BeginCheckInTime
    ,EndCheckInTime
    ,BeginCheckOutTime
    ,EndCheckOutTime
    ,IsInAfterDay
    ,IsOutAfterDay
    ,IsDisabled
    FROM dbo.[Shift] WITH (NOLOCK)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Shift_GetShifts]
AS
/*
Created: 20141002@Quinn.Vo
Task: Get all shifts
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON;

    SELECT [Id]
      ,[Code]
      ,[Name]
      ,[BeginCheckInTime]
      ,[EndCheckInTime]
      ,[BeginCheckOutTime]
      ,[EndCheckOutTime]
      ,[BreakTimeMins]
      ,[OrderNo]
      ,[BeginBreakTime]
      ,[EndBreakTime]
      ,[IsInAfterDay]
      ,[IsOutAfterDay]
      ,[ModifiedBy]
      ,[ModifiedDate]
      ,[IsDisabled]
    FROM dbo.[Shift]
    WHERE IsDisabled = 0
    ORDER BY OrderNo
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SkypeAccount_GetAllEmployee]
	@EmployeeId INT = NULL
	,@GroupId INT = NULL
	,@IsConfirmed INT = NULL

AS
/*
Created: 20180725@Oliver.Dang
Task: Get all Employee with Skype account map from Nexbot
DB: NexHR & NexBot
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT u.ConversationId
		  ,e.Id AS EmpId
		  ,e.Code 
		  ,e.FullName
		  ,e.NickName
		  ,g.Id AS GroupId
		  ,g.Code
		  ,g.Name
	FROM [dbo].Employee AS e WITH (NOLOCK)
	LEFT JOIN [dbo].[EmployeeGroup] AS g WITH (NOLOCK) ON e.GroupId = g.Id
	LEFT JOIN [dbo].EmployeeConversation AS u WITH (NOLOCK) ON e.iD = u.EmpId
	WHERE e.[Enabled] = 1 AND e.isDeleted = 0
		AND (@EmployeeId IS NULL OR @EmployeeId < 0 OR e.Id = @EmployeeId)
		AND (@GroupId IS NULL OR g.Id = @GroupId)
		AND (@IsConfirmed IS NULL
			 OR ((CASE WHEN u.EmpId IS NULL THEN 0 ELSE 1 END) = @IsConfirmed))
	ORDER BY g.[Rank]
			,g.Code
			,e.Code
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Supervisor_Get] 
    @keyword NVARCHAR(256),
    @groupId INT = NULL,
    @departmentId INT = NULL
AS
/*
Created: 20240516@Lilac.Nguyen
Task: Get Supervisor detail.
DB: NexHRM
Original: 

Revisions:  
    20250516@Henry.Dinh: Add filter by department ID
Param's Explanation:
    @keyword: Search keyword for employee name
    @groupId: Filter by group ID
    @departmentId: Filter by department ID
*/
BEGIN
    SET NOCOUNT ON;

	DECLARE @q NVARCHAR(256)

    SET @q = '%' + @keyword + '%'

    IF LEN(@keyword) = 0
        SET @keyword = NULL

	SELECT
		e.Id AS EmpId,
		e.NickName,
		e.FullName,
		d.[Name] AS DepartmentName,
		g.[Name] AS GroupName,
		STRING_AGG(CONVERT(NVARCHAR(MAX), sup.ApproverNickName), ', ') AS Approvers
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeSupDetail AS sup WITH (NOLOCK) ON sup.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g ON e.GroupId = g.Id
		LEFT JOIN dbo.Department AS d ON g.DepartmentId = d.DepartmentId
	WHERE 
		ISNULL(e.IsDeleted, 0) = 0
		AND e.[Enabled] <> 0 
		AND e.IsSpecialUser = 0
		AND (
			@keyword IS NULL
			OR e.NickName LIKE @q
			OR e.FullName LIKE @q
		)
		AND (
			@groupId IS NULL
			OR e.GroupId = @groupId
		)
		AND (
			@departmentId IS NULL
			OR g.DepartmentId = @departmentId
		)
	GROUP BY 
		e.Id,
		e.NickName,
		e.FullName,
		d.Name,
		g.[Name],
		g.[Rank],
		g.Code,
		e.Roles,
		e.GroupDate
	ORDER BY 
		d.[Name],
		g.[Rank],
		g.Code,
		e.Roles DESC,
		e.GroupDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_Employee_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT e.Id AS EmployeeId, 
		e.DomainUsername, 
		e.Code, 
		e.FullName, 
		e.NickName, 
		e.GroupId, 
		e.[Enabled], 
		e.isDeleted, 
		CAST(e.JoinedDate AS DATE) AS JoinedDate, 
		CAST(e.ConfirmedDate AS DATE) 
        AS ConfirmedDate, 
		e.JobTitleId, 
		e.Email, 
		ec.ConversationId,
		e.ModifiedDate
	FROM dbo.Employee AS e WITH (NOLOCK) LEFT OUTER JOIN
        dbo.EmployeeConversation AS ec WITH (NOLOCK) ON e.Id = ec.EmpId
	WHERE e.ModifiedDate > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_EmployeeAttendance_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee attendence
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON

	SELECT Id,
		EmpId, 
		AttendanceDate, 
		InFixed, 
		LeaveDay, 
		OutFixed, 
		LateIn,
		EarlyOut, 
		LeaveTypeId, 
		LeaveTypeId2, 
		Absence, 
		TotalWorkHours,
		InsertDate
	FROM dbo.EmployeeAttendance WITH (NOLOCK)
	WHERE InsertDate > @LastModifiedDate
	
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_EmployeeCalendar_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee calendar
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT Id,
		EmpId, 
		WorkDate, 
		IsOffDay, 
		IsHoliday,
		IsLeaveDay, 
		LeaveDay,
		ModifiedDate
	FROM dbo.EmployeeCalendar WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_EmployeeOverTime_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee ot by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT ot.OvertimeId,
		otd.EmployeeId, 
		otd.[Date], 
		otd.[Hour],
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM dbo.Overtime AS ot WITH (NOLOCK) 
	INNER JOIN dbo.OvertimeDetail AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
	WHERE ISNULL(ot.ModifiedDate, ot.CreatedDate) > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_EmployeeTOEIC_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get employee toeic by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT EmpId, 
		MinimumScore, 
		CAST(InputDate AS DATE) AS InputDate, 
		CAST(LRExamDate AS DATE) AS LRExamDate, 
		Listening, 
		Reading,
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM HRM.dbo.TOEIC WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_GO_UnitHead_GetInfo]
AS
/*
Created: 20230307@Tim.Trinh
Task: get unit head info
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.Id AS EmpId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,emp.[Enabled] AS IsActive
		,emp.IsDeleted
		,gr.Code AS DepartmentCode
		,gr.[Name] AS DepartmentName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS gr WITH (NOLOCK) ON emp.GroupId = gr.Id
	WHERE emp.Code IN ('02197')
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_GO_UnitHead_GetSchedule] @FromDate DATE
	,@ToDate DATE
AS
/*
Created: 20230307@Tim.Trinh
Task: get unit head schedule
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tempUnitHead (EmpId INT PRIMARY KEY)

	INSERT INTO #tempUnitHead (EmpId)
	SELECT emp.Id
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE emp.Code IN ('02197')
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0

	SELECT empca.EmpId
		,empca.WorkDate
		,CASE 
			WHEN empca.IsLeaveDay = 0
				AND empca.ShiftId IS NOT NULL THEN 1
			ELSE 0
			END AS IsWorkday
		,CASE 
			WHEN empca.IsLeaveDay = 1
				OR empca.ShiftId IS NOT NULL
				OR empca.IsHoliday = 1 THEN 0
			ELSE empca.IsOffDay
			END AS IsOffDay
		,CASE 
			WHEN empca.IsLeaveDay = 1
				OR empca.ShiftId IS NOT NULL THEN 0
			ELSE empca.IsHoliday
			END AS IsHoliday
		,empca.IsLeaveDay
		,CASE 
			WHEN empca.LeaveTypeId IS NOT NULL THEN ltype.Code
			WHEN empca.LeaveTypeId2 IS NOT NULL THEN ltype2.Code
			END AS LeaveTypeCode
		,CASE 
			WHEN empca.LeaveTypeId IS NOT NULL THEN ltype.[Name]
			WHEN empca.LeaveTypeId2 IS NOT NULL THEN ltype2.[Name]
			END AS LeaveTypeName
	FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
		LEFT JOIN dbo.LeaveType AS ltype WITH (NOLOCK) ON empca.LeaveTypeId = ltype.Id
		LEFT JOIN dbo.LeaveType AS ltype2 WITH (NOLOCK) ON empca.LeaveTypeId2 = ltype2.Id
	WHERE empca.WorkDate BETWEEN @FromDate
			AND @ToDate
		AND EXISTS (
			SELECT 1
			FROM #tempUnitHead AS tmp WITH (NOLOCK)
			WHERE empca.EmpId = tmp.EmpId
			)

	DROP TABLE #tempUnitHead
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_Group_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get group by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT Id AS GroupId, 
		[Name], 
		Code, 
		ParentId, 
		CASE 
			WHEN DepartmentId IS NULL OR DepartmentId = 0 
				THEN 999 
			WHEN DepartmentId IS NOT NULL 
				THEN DepartmentId 
		END AS DepartmentId, 
		ISNULL(Deleted, 0) AS IsDeleted, 
		[Rank],
		ParentId,
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM dbo.EmployeeGroup WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_JobTitle_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get job title by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT JobTitleId AS Id, 
		[Name],
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM  HRM.dbo.JobTitle WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Sync_WorkingTimeRequest_Get] @LastModifiedDate DATETIME
AS
/*
Created: Jay.Do@20250610
Task: Get working time request by last modified date
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
    SET NOCOUNT ON
	
	SELECT Id,
		EmpId, 
		RequestType, 
		FromDate, 
		ToDate, 
		TotalDays, 
		[Status],
		ISNULL(ModifiedDate, CreatedDate) AS ModifiedDate
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, CreatedDate) > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SyncData_GetDepartmentByLastModifiedDate] @LastModifiedDate DATETIME
AS
/*
Created: 20201126@Oliver.Dang
Task: Get Employee to sync
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;

	SELECT d.DepartmentId
		,d.Code
		,d.Name
		,d.ParentId
		,d.ManagerId
		,d.DepartmentLevel
		,(
			CASE 
				WHEN ISNULL(d.IsDisabled, 0) = 1
					THEN 0
				ELSE 1
				END
			) AS IsActive
		,d.ModifiedDate
	FROM dbo.Department d WITH (NOLOCK)
	WHERE d.ModifiedDate IS NULL OR d.ModifiedDate > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SyncData_GetEmployeeByLastModifiedDate]
	@LastModifiedDate DATETIME
AS
/*
Created: 20201126@Oliver.Dang
Task: Get Employee to sync
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT e.Id as SourceEmpId
		  ,e.DomainUsername
		  ,e.Code
		  ,e.NickName
		  ,e.FullName
		  ,e.JobTitleId
		  ,e.GroupId
		  ,e.[Enabled]
		  ,e.isDeleted
		  ,e.IsSpecialUser
		  ,e.Email
		  ,e.JoinedDate
		  ,e.ConfirmedDate
		  ,e.ResignedDate
		  ,e.ModifiedDate 
	FROM dbo.Employee e WITH (NOLOCK)
	WHERE e.ModifiedDate > @LastModifiedDate

END
GO

CREATE OR ALTER PROCEDURE [dbo].[SyncData_GetGroupByLastModifiedDate] @LastModifiedDate DATETIME
AS
/*
Created: 20201126@Oliver.Dang
Task: Get Employee to sync
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;

	SELECT g.Id as GroupId
		  ,g.Code
		  ,g.Name
		  ,g.DepartmentId
		  ,g.[Rank]
		  ,ISNULL(g.Deleted,0) as IsDeleted
		  ,g.ModifiedDate
	FROM dbo.EmployeeGroup g WITH (NOLOCK)
	WHERE g.ModifiedDate > @LastModifiedDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SyncData_GetJobTitle]
AS
/*
Created: 20201130@Oliver.Dang
Task: Get Jobtitle to sync
DB: NexHRM
Original: 

*/
BEGIN
	SET NOCOUNT ON;

	SELECT jt.JobTitleId
		  ,jt.Name
	FROM HRM.dbo.JobTitle jt WITH (NOLOCK)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[System_GetErrorInfo]
AS
    SELECT 
        ERROR_NUMBER() AS ErrorNumber
        ,ERROR_SEVERITY() AS ErrorSeverity
        ,ERROR_STATE() AS ErrorState
        ,ERROR_LINE () AS ErrorLine
        ,ERROR_PROCEDURE() AS ErrorProcedure
        ,ERROR_MESSAGE() AS ErrorMessage;
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_EditInsertCheckInOutTime] @EmpId INT
	,@RecordTime DATETIME
	,@InOutType TINYINT
	,@Platform VARCHAR(255)
	,@IPAddress VARCHAR(50)
	,@IsEditCheckInOutOnline BIT
	,@ModifiedBy INT
	,@ModifiedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20220104@Teddy.le
Task: Edit Insert Check In/Out Time.
DB: HRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
	DECLARE @ToDate DATETIME
	DECLARE @RecordDate DATETIME
	DECLARE @TimeRecoredId INT
	DECLARE @Status TINYINT = 3

	INSERT INTO [dbo].[TimeRecord] (
		EmpId
		,RecordTime
		,InOutType
		,InsertDate
		,ModifiedBy
		,ModifiedDate
		)
	VALUES (
		@EmpId
		,@RecordTime
		,CASE WHEN @InOutType = 0 THEN 0 ELSE 1 END
		,dbo.fnc_GetClientDateTime()
		,@ModifiedBy
		,@ModifiedDate
		)

	SET @RecordDate = convert(VARCHAR(10), @RecordTime, 111)

	SELECT @TimeRecoredId = CAST(SCOPE_IDENTITY() AS INT)

	SELECT @Status = Status
	FROM dbo.WorkFromHomeRequest
	WHERE EmpId = @EmpId
	AND @RecordDate BETWEEN FromDate AND ToDate

	INSERT INTO dbo.TimeRecordExt(ID, IPAddress, [Platform], [Status], [IsEditCheckInOutOnline])
	VALUES(@TimeRecoredId, @IPAddress, @Platform, @Status, @IsEditCheckInOutOnline)

	SET @FromDate = convert(VARCHAR(10), dateadd(d, - 2, @RecordTime), 111)
	SET @ToDate = convert(VARCHAR(10), @RecordTime, 111)

	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
		,@FromDate
		,@ToDate
		,1
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_Get] @EmpID INT = NULL
	,@fromDate DATETIME
	,@toDate DATETIME
	,@groupIds VARCHAR(MAX) = ''
	,@PageIndex INT = NULL
	,@ItemsPerPage INT = NULL
	,@TotalItem INT = NULL OUT
AS
/*
Created: Quinn.Vo@2014-01-24
Task: Get Time Record
DB: NexHRM
Original: 

Teddy.le@20211222
Task: Add Is check in/out online and IP address

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	IF OBJECT_ID('tempdb..#TimeRecord') IS NOT NULL
	BEGIN
		DROP TABLE #TimeRecord
	END

	CREATE TABLE #TimeRecord (
		RecordTimeId INT
		,RecordTime DATETIME
		,InOutType TINYINT
		,EmpId INT
		,Code VARCHAR(20)
		,NickName NVARCHAR(20)
		,FullName NVARCHAR(50)
		,GroupId INT
		,GroupCode VARCHAR(20)
		,GroupName NVARCHAR(50)
		,ReaderName NVARCHAR(64)
		,EventName NVARCHAR (64)
		,IsCheckInOutOnline BIT
		,IPAddress NVARCHAR (50)
		,Status TINYINT
		,IsEditCheckInOutOnline BIT
		)

	DECLARE @GroupsList TABLE (GroupId INT)

	INSERT INTO @GroupsList (GroupId)
	SELECT val
	FROM fnc_strSplit(@groupids, ',')

	INSERT INTO #TimeRecord
	(
		RecordTimeId
		,RecordTime
		,InOutType
		,ReaderName
		,EventName
		,IsCheckInOutOnline
		,IPAddress
		,Status
		,IsEditCheckInOutOnline
		,EmpId 
		,Code
		,NickName
		,FullName
		,GroupId 
		,GroupCode
		,GroupName
	)
	SELECT t.[Id]
		,t.RecordTime
		,ISNULL(t.InOutType, 2)
		,CASE WHEN bio.DeviceId IS NULL THEN t.ReaderName ELSE bio.DisplayName END as ReaderName
		,t.EventName as EventName
		,CASE
			WHEN tex.ID IS NOT NULL THEN 1
			ELSE 0
		END AS IsInOutOnline
		,tex.IPAddress
		,tex.Status
		,tex.IsEditCheckInOutOnline
		,e.Id
		,e.Code
		,e.NickName
		,e.FullName
		,g.Id
		,g.Code
		,g.[Name]
	FROM dbo.TimeRecord AS t WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON t.EmpID = e.Id
		INNER JOIN dbo.[EmployeeGroup] AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN HRM.dbo.BioDevice AS bio WITH (NOLOCK) ON t.ReaderId = bio.DeviceId
		LEFT JOIN dbo.TimeRecordExt AS tex WITH (NOLOCK) ON t.ID = tex.ID
	WHERE CONVERT(VARCHAR(10), RecordTime, 111) BETWEEN CONVERT(VARCHAR(10), @fromDate, 111)
			AND CONVERT(VARCHAR(10), @toDate, 111)
		AND (
			@EmpID IS NULL
			OR e.Id = @EmpID
			)
		AND (
			ISNULL(@groupIds, '') = ''
			OR e.GroupId IN (
				SELECT GroupId
				FROM @GroupsList
				)
			)
	ORDER BY t.RecordTime DESC

	IF @PageIndex IS NOT NULL
		AND @ItemsPerPage IS NOT NULL
	BEGIN
		SELECT t.RecordTimeId
		,t.RecordTime
		,ISNULL(t.InOutType, 2) as InOutType
		,t.ReaderName as ReaderName
		,t.EventName as EventName
		,t.IsCheckInOutOnline
		,t.IPAddress
		,t.Status
		,t.IsEditCheckInOutOnline
		,t.EmpId
		,t.Code
		,t.NickName
		,t.FullName
		,t.GroupId
		,t.GroupCode
		,t.GroupName
		FROM #TimeRecord AS t
		ORDER BY t.RecordTime DESC OFFSET(@PageIndex * @ItemsPerPage) ROWS

		FETCH NEXT @ItemsPerPage ROWS ONLY;
	END
	ELSE
	BEGIN
		SELECT t.RecordTimeId
		,t.RecordTime
		,ISNULL(t.InOutType, 2) as InOutType
		,t.ReaderName as ReaderName
		,t.EventName as EventName
		,t.IsCheckInOutOnline
		,t.IPAddress
		,t.Status
		,t.IsEditCheckInOutOnline
		,t.EmpId
		,t.Code
		,t.NickName
		,t.FullName
		,t.GroupId
		,t.GroupCode
		,t.GroupName
		FROM #TimeRecord AS t
		ORDER BY t.RecordTime DESC
	END

	SELECT @TotalItem = COUNT(*)
	FROM #TimeRecord

	IF OBJECT_ID('tempdb..#TimeRecord') IS NOT NULL
	BEGIN
		DROP TABLE #TimeRecord
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_GetPrivilegeCreateCheckInOut] @EmpId INT
AS
/*
Created: 20211213@Teddy.le
Task: Get Privilege Create Check In/Out Record.
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsPrivilegeCreateCheckInOut BIT = 0,
		@Today DATETIME
		
	SET @Today = CONVERT(date, dbo.fnc_GetClientDateTime())

	IF (EXISTS(SELECT 1 FROM dbo.WorkFromHomeRequest WITH (NOLOCK) 
		WHERE EmpId = @EmpId 
			AND @Today BETWEEN FromDate AND ToDate
			AND Status <> 3
		))
	BEGIN
		SET @IsPrivilegeCreateCheckInOut = 1
	END

	SELECT @IsPrivilegeCreateCheckInOut
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_ImportBioStationTimeChange]
AS
/*
Created: 20180626@Quinn
Task: Import timechange events from BioStation database
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @FromDate DATETIME = DATEADD(DD, -30, dbo.fnc_GetClientDateTime())

    CREATE TABLE #EventLog
    (
        nEventLogIdn INT
	    ,nDateTime DATETIME
	    ,nReaderIdn INT
	    ,nEventIdn INT
    )

    INSERT INTO #EventLog
    (
        nEventLogIdn
        ,nDateTime
        ,nEventIdn
        ,nReaderIdn
    )
    SELECT nEventLogIdn
	    ,DATEADD(s, nDateTime, '1970-01-01')
	    ,nEventIdn
	    ,nReaderIdn
    FROM [BioStar].dbo.TB_EVENT_LOG WITH (NOLOCK)
    WHERE DATEADD(s, nDateTime, '1970-01-01') > @FromDate

	INSERT INTO [dbo].BioStationTimeChange
	(
	   [nEventLogIdn]
      ,[DeviceId]
      ,[TimeChange]
      ,[PreviousLogTime]
      ,[NextLogTime]
      ,[InsertedDate]
	)
    SELECT con.nEventLogIdn
		,con.nReaderIdn AS DeviceId
	    ,con.nDateTime AS TimeChange
	    ,pre.nDateTime AS PreviousLogTime
	    ,nex.nDateTime AS NextLogTime
		,dbo.fnc_GetClientDateTime()
    FROM (
	SELECT timechanges.nEventLogIdn
		,(
			SELECT MIN(ee.nEventLogIdn)
            FROM #EventLog AS ee
            WHERE timechanges.nReaderIdn <> ee.nReaderIdn
                AND timechanges.nEventLogIdn < ee.nEventLogIdn
			) AS NexId
		,(
			SELECT MAX(ee.nEventLogIdn)
            FROM #EventLog AS ee
            WHERE timechanges.nReaderIdn <> ee.nReaderIdn
                AND timechanges.nEventLogIdn > ee.nEventLogIdn
			) AS PreId
        FROM #EventLog AS timechanges
        WHERE timechanges.nEventIdn = 210
	) NextPre
        INNER JOIN #EventLog AS con ON NextPre.nEventLogIdn = con.nEventLogIdn
        INNER JOIN #EventLog AS nex ON NextPre.NexId = nex.nEventLogIdn
        INNER JOIN #EventLog AS pre ON NextPre.PreId = pre.nEventLogIdn
	WHERE NOT EXISTS (
		SELECT 1 FROM dbo.BioStationTimeChange WHERE nEventLogIdn = con.nEventLogIdn
	)
    ORDER BY con.nEventLogIdn

    DROP TABLE #EventLog
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_ImportData]
(
    @TimeRecordXml NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
)
AS
/*
    Created: 20160927
    Task: import time record
    DB: NexHR

    Revisions:    
    Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Today DATETIME
        ,@Yesterday DATETIME
		,@TempDate DATETIME
        ,@DocHandler INT

	SELECT @Today = dbo.fnc_GetClientDateTime()

    SELECT @Yesterday = DATEADD(dd, - 1, @Today)

	CREATE TABLE #tempRecord (
		EmpNo VARCHAR(32)
		,RecordTime DATETIME
		,InOutType TINYINT
		,ReaderId INT
		,ReaderName NVARCHAR(48)
		,EventName NVARCHAR(100)
		)

    CREATE TABLE #tempTimeRecord (
		EmpId INT
		,TimeRecord DATETIME
		)

    EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@TimeRecordXml

    INSERT INTO #tempRecord (
		EmpNo
		,RecordTime
		,InOutType
		,ReaderId
		,ReaderName
		,EventName
		)
    SELECT EmpNo
		,RecordTime
		,InOutType
		,ReaderId
		,ReaderName
		,EventName
    FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (
        EmpNo VARCHAR(32)
		,RecordTime DATETIME
		,InOutType TINYINT
		,ReaderId INT
		,ReaderName NVARCHAR(48)
		,EventName NVARCHAR(100)) AS d

    EXEC sp_xml_removedocument @DocHandler

    INSERT #tempTimeRecord(EmpId, TimeRecord)
	SELECT e.Id
		,f.RecordTime
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #tempRecord AS f ON ISNULL(TRY_PARSE(e.Code AS INT), 0) = ISNULL(TRY_PARSE(f.EmpNo AS INT), -1)
	WHERE e.isDeleted = 0
		AND	NOT EXISTS (
			SELECT 1
			FROM dbo.TimeRecord AS t WITH (NOLOCK)
			WHERE t.EmpId = e.Id
				AND DATEDIFF(SECOND, RecordTime, f.RecordTime) = 0
			)

    INSERT dbo.TimeRecord (
		EmpId
		,RecordTime
		,InOutType
		,InsertDate
		,ReaderId
		,ReaderName
		,EventName
		)
	SELECT e.Id
		,f.RecordTime
		,f.InOutType
		,dbo.fnc_GetClientDateTime()
		,f.ReaderId
		,f.ReaderName
		,f.EventName
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #tempRecord f ON ISNULL(TRY_PARSE(e.Code AS INT), 0) = ISNULL(TRY_PARSE(f.EmpNo AS INT), -1)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.TimeRecord t WITH (NOLOCK)
			WHERE t.EmpId = e.Id
				AND DATEDIFF(SECOND, t.RecordTime, f.RecordTime) = 0
			)

    INSERT INTO dbo.tmpEmployeeAttendance(EmpId, WorkDate, InsertedDate)
	SELECT t.EmpId
		,CAST(TimeRecord AS DATE)
		,dbo.fnc_GetClientDateTime()
	FROM #tempTimeRecord t
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.tmpEmployeeAttendance WITH (NOLOCK) 
		WHERE EmpId = t.EmpId 
		AND WorkDate = CAST(DATEADD(DD, -1, t.TimeRecord) AS DATE)
    )

	UNION
	SELECT t.EmpId 
		,CAST(TimeRecord AS DATE)
		,dbo.fnc_GetClientDateTime()
	FROM #tempTimeRecord t
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.tmpEmployeeAttendance WITH (NOLOCK) 
		WHERE EmpId = t.EmpId 
		AND WorkDate = CAST(t.TimeRecord AS DATE)
    )

	EXEC dbo.EmployeeAttendance_ProcessingData

	IF EXISTS (
        SELECT 1 
		FROM dbo.Employee AS e WITH (NOLOCK) 
		    LEFT JOIN dbo.EmployeeAttendance AS a WITH (NOLOCK) ON e.Id = a.EmpId AND DATEDIFF(DD, AttendanceDate, @Today) = 0
		WHERE e.isDeleted = 0
            AND e.Enabled = 1 
		    AND e.IsSpecialUser = 0
		    AND a.EmpId IS NULL
    )
	BEGIN
		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@Yesterday
			,@Yesterday
			,0

		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@Today
			,@Today
			,0
	END

	SELECT @TempDate = DATEADD(MM, 2, dbo.fnc_GetClientDateTime())

	IF (DATEDIFF(MI, CAST(@Today AS DATE), @Today) < 5)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM dbo.Employee AS e WITH (NOLOCK)
					LEFT JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON e.Id = cl.EmpId
					AND WorkDate = CAST(@TempDate AS DATE)
					AND ISNULL(e.Enabled, 0) = 1
					AND ISNULL(e.isDeleted, 0) = 0
				)
		BEGIN
			EXEC dbo.EmployeeCalendar_Insert NULL
				,@TempDate
				,@TempDate
		END
	END

	DROP TABLE #tempRecord
	DROP TABLE #tempTimeRecord
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_ImportFromBioAdmin] @fromDate DATETIME
	,@toDate DATETIME
AS
/*
Created: 2012/03/13@Dan
Task: Import data from BioAdmin to HRSmart system
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
• @importdate: the date to import data
*/
BEGIN
	SET NOCOUNT ON
	DECLARE @Yesterday DATETIME
	--SET @Yesterday = DATEADD(DD, -1, dbo.fnc_GetClientDateTime())
	SET @Yesterday =  dbo.fnc_GetClientDateTime()

	-- get data from BioAdmin
	SELECT DATEADD(s, nDateTime, '1970-01-01') AS dtDateTime
		,nTNAEvent
		,nUserID
		,CASE WHEN rd.sName IS NULL THEN CAST(evl.nReaderIdn AS NVARCHAR(64)) ELSE rd.sName END ReaderName
		,eda.sName AS EventName
		,rd.nReaderIdn AS ReaderId
	INTO #tempFinger
	FROM [BioStar].dbo.TB_EVENT_LOG AS evl WITH (NOLOCK)
	INNER JOIN [BioStar].[dbo].[TB_EVENT_DATA] AS eda WITH (NOLOCK) ON evl.nEventIdn = eda.nEventIdn
	INNER JOIN [BioStar].[dbo].[TB_USER] AS us WITH (NOLOCK) ON us.sUserID = evl.nUserID
	LEFT JOIN [BioStar].[dbo].[TB_READER] AS rd WITH (NOLOCK) ON evl.nReaderIdn = rd.nReaderIdn
	WHERE DATEADD(s, nDateTime, '1970-01-01') >= @fromDate
		AND DATEADD(s, nDateTime, '1970-01-01') <= dateadd(d, 1, @toDate) -- not use datediff because it does not use index
		AND nUserID > 0
		AND evl.nEventIdn IN (36, 38, 39, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 58, 59, 60, 61, 62, 98, 99, 119)
		AND ISNULL(us.nDepartmentIdn , 0) <> 13
		--AND nEventIdn IN (47, 58)
	ORDER BY dtDateTime
	
	IF (
			EXISTS (
				SELECT 1
				FROM dbo.Employee e WITH (NOLOCK)
					JOIN #tempFinger f ON e.Code IS NOT NULL
					AND CAST(e.Code AS DECIMAL(18, 0)) = f.nUserID
				WHERE NOT EXISTS (
						SELECT 1
						FROM dbo.TimeRecord t WITH (NOLOCK)
						WHERE t.empID = e.ID
							AND datediff(second, RecordTime, f.dtDateTime) = 0
						)
				)
			)
	BEGIN
		-- import check-in/out data to TS_TimeRecord 
		INSERT TimeRecord (
			EmpID
			,RecordTime
			,InOutType
			,InsertDate
			,ReaderName
			,EventName
			,ReaderId
			)
		SELECT e.ID
			,dtDateTime
			,CASE 
				WHEN (
						nTNAEvent = 0
						OR nTNAEvent = 2
						) THEN 0
				WHEN (
						nTNAEvent = 1
						OR nTNAEvent = 3
						) THEN 1
				ELSE NULL
				END
			,dbo.fnc_GetClientDateTime()
			,ReaderName
			,EventName
			,ReaderId
		FROM dbo.Employee e WITH (NOLOCK)
			JOIN #tempFinger f ON e.Code IS NOT NULL
			AND CAST(e.Code AS DECIMAL(18, 0)) = f.nUserID
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.TimeRecord t WITH (NOLOCK)
				WHERE t.empID = e.ID
					AND datediff(second, RecordTime, f.dtDateTime) = 0
				)
		ORDER BY dtDateTime

		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@fromDate
			,@toDate
			,0
	END
	ELSE IF(NOT EXISTS (SELECT 1 FROM dbo.EmployeeAttendance WITH (NOLOCK) WHERE DATEDIFF(DD, AttendanceDate, @Yesterday) = 0))
	BEGIN
		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@Yesterday
			,@Yesterday
			,0
	END
	DROP TABLE #tempFinger
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_ImportFromBioStar]
	@fromDate DATETIME
	,@toDate DATETIME
AS
/*
Created: 20170914@QuinnVo
Task: Import data from BioStar to HR system
DB: HRM
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @SQLString NVARCHAR(4000)
		,@CurrentMonthTableName VARCHAR(50)
		,@LastMonthTableName VARCHAR(50)
		,@fromDateString VARCHAR(10)
		,@toDateString VARCHAR(10)
		,@StartDateConnectBitoStar VARCHAR(10)

	DECLARE @toDay DATETIME, @Yesterday DATETIME
	SET @toDay =  dbo.fnc_GetClientDateTime()
	SET @Yesterday = DATEADD(DD, -1, @toDay)

	CREATE TABLE #tempRecord (
		nUserID VARCHAR(32)
		,dtDateTime DATETIME
		,nTNAEvent INT
		,ReaderId INT
		,ReaderName NVARCHAR(48)
		,EventName NVARCHAR(100)
		)

	CREATE TABLE #tempTimeRecord (
		EmpId INT
		,TimeRecord DATETIME
		)

	SELECT @CurrentMonthTableName = 'T_LG' + LEFT(CONVERT(VARCHAR, dbo.fnc_GetClientDateTime(), 112), 6)
		,@LastMonthTableName = 'T_LG' + LEFT(CONVERT(VARCHAR, DATEADD(MM, - 1, dbo.fnc_GetClientDateTime()), 112), 6)
		,@fromDateString = CONVERT(VARCHAR, @fromDate, 112)
		,@toDateString = CONVERT(VARCHAR, @toDate, 112)
		,@StartDateConnectBitoStar = '2022-10-01'

	IF EXISTS (
			SELECT 1
			FROM [ACDB].[dbo].[T_LGTBIDX]
			WHERE [LGTABLENM] = @LastMonthTableName
			)
	BEGIN
		
		SET @SQLString = N'INSERT INTO #tempRecord(nUserID, dtDateTime, nTNAEvent, ReaderId, ReaderName, EventName) 
						SELECT 
						a.USRID AS nUserID
						  ,DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) AS dtDateTime
						  ,a.[TNAKEY] AS nTNAEvent
						  ,b.[DEVID]
						  ,b.[NM]
						  ,c.NM
					FROM [ACDB].[dbo].' + @LastMonthTableName + ' AS a
					LEFT JOIN [ACDB].[dbo].[T_DEV] AS b ON a.DEVUID = b.[DEVID]
					LEFT JOIN [ACDB].[dbo].[T_EVTTYP] AS c ON a.EVT = c.EVT
					WHERE a.USRID <> '''' AND a.EVT < 5400 AND a.USRGRUID <> 2250
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) > ''' + @fromDateString + '''
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) < DATEADD(d, 1, ''' + @toDateString + ''')
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) > ''' + @StartDateConnectBitoStar + ''''

		EXECUTE sp_executesql @SQLString

	END

	IF EXISTS (
			SELECT 1
			FROM [ACDB].[dbo].[T_LGTBIDX]
			WHERE [LGTABLENM] = @CurrentMonthTableName
			)
	BEGIN
		SET @SQLString = N'INSERT INTO #tempRecord(nUserID, dtDateTime, nTNAEvent, ReaderId, ReaderName, EventName) 
						SELECT 
					 a.USRID AS nUserID
						  ,DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) AS dtDateTime
						  ,a.[TNAKEY] AS nTNAEvent
						  ,b.[DEVID]
						  ,b.[NM]
						  ,c.NM
	  
					FROM [ACDB].[dbo].' + @CurrentMonthTableName + ' AS a 
					LEFT JOIN [ACDB].[dbo].[T_DEV] AS b ON a.DEVUID = b.[DEVID]
					LEFT JOIN [ACDB].[dbo].[T_EVTTYP] AS c ON a.EVT = c.EVT
					WHERE a.USRID <> '''' AND a.EVT < 5400 AND a.USRGRUID <> 2250
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) > ''' + @fromDateString + '''
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) < DATEADD(d, 1, ''' + @toDateString + ''')
					AND DATEADD(HH, 7, DATEADD(s, a.[DEVDT], ''1970-01-01'')) > ''' + @StartDateConnectBitoStar + ''''

		EXECUTE sp_executesql @SQLString

	END

	INSERT #tempTimeRecord(EmpId, TimeRecord)
	SELECT e.Id
		,dtDateTime
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #tempRecord f ON ISNULL(TRY_PARSE(e.Code AS INT), 0) = ISNULL(TRY_PARSE(f.nUserID AS INT), -1)
	WHERE e.isDeleted = 0
		AND	NOT EXISTS (
			SELECT 1
			FROM dbo.TimeRecord t WITH (NOLOCK)
			WHERE t.empID = e.Id
				AND DATEDIFF(SECOND, RecordTime, f.dtDateTime) = 0
			)

	INSERT TimeRecord (
		EmpID
		,RecordTime
		,InOutType
		,InsertDate
		,ReaderId
		,ReaderName
		,EventName
		)

	SELECT e.Id
		,dtDateTime
		,CASE 
			WHEN nTNAEvent = 1 THEN 0
			WHEN nTNAEvent = 2 THEN 1
			ELSE NULL
			END
		,dbo.fnc_GetClientDateTime()
		,f.ReaderId
		,f.ReaderName
		,f.EventName
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #tempRecord f ON ISNULL(TRY_PARSE(e.Code AS INT), 0) = ISNULL(TRY_PARSE(f.nUserID AS INT), -1)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.TimeRecord t WITH (NOLOCK)
			WHERE t.empID = e.Id
				AND DATEDIFF(SECOND, RecordTime, f.dtDateTime) = 0
			)

	INSERT INTO dbo.tmpEmployeeAttendance(EmpId, WorkDate, InsertedDate)
	SELECT t.EmpId
		,CAST(TimeRecord AS DATE)
		,dbo.fnc_GetClientDateTime()
	FROM #tempTimeRecord t
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.tmpEmployeeAttendance WITH (NOLOCK) 
		WHERE EmpId = t.EmpId 
		AND WorkDate = CAST(DATEADD(DD, -1, t.TimeRecord) AS DATE))
	UNION
	SELECT t.EmpId 
		,CAST(TimeRecord AS DATE)
		,dbo.fnc_GetClientDateTime()
	FROM #tempTimeRecord t
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.tmpEmployeeAttendance WITH (NOLOCK) 
		WHERE EmpId = t.EmpId 
		AND WorkDate = CAST(t.TimeRecord AS DATE))

	EXEC dbo.EmployeeAttendance_ProcessingData

	IF(EXISTS (SELECT 1 
		FROM dbo.Employee AS e WITH (NOLOCK) 
		LEFT JOIN dbo.EmployeeAttendance AS a WITH (NOLOCK) ON e.Id = a.EmpId AND DATEDIFF(DD, AttendanceDate, @toDate) = 0
		WHERE e.isDeleted = 0 AND e.Enabled = 1 
		AND e.IsSpecialUser = 0
		AND a.EmpId IS NULL))
	BEGIN
		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@Yesterday
			,@Yesterday
			,0

		EXEC EmployeeAttendance_ProcessingAndInsert NULL
			,@toDay
			,@toDay
			,0
	END

	DROP TABLE #tempRecord
	DROP TABLE #tempTimeRecord
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_InsertCheckInOutTime] @EmpId INT
	,@RecordTime DATETIME
	,@InOutType TINYINT
	,@Platform VARCHAR(255)
	,@IPAddress VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211213@Teddy.le
Task: Insert Check In/Out Time.
DB: HRM
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
	DECLARE @ToDate DATETIME
	DECLARE @RecordDate DATETIME
	DECLARE @TimeRecoredId INT
	DECLARE @Status TINYINT = 3

	INSERT INTO [dbo].[TimeRecord] (
		EmpId
		,RecordTime
		,InOutType
		,InsertDate
		)
	VALUES (
		@EmpId
		,@RecordTime
		,CASE WHEN @InOutType = 0 THEN 0 ELSE 1 END
		,dbo.fnc_GetClientDateTime()
		)

	SET @RecordDate = convert(VARCHAR(10), @RecordTime, 111)

	SELECT @TimeRecoredId = CAST(SCOPE_IDENTITY() AS INT)

	SELECT @Status = Status
	FROM dbo.WorkFromHomeRequest
	WHERE EmpId = @EmpId
	AND @RecordDate BETWEEN FromDate AND ToDate

	INSERT INTO dbo.TimeRecordExt(ID, IPAddress, [Platform], [Status])
	VALUES(@TimeRecoredId, @IPAddress, @Platform, @Status)

	SET @FromDate = convert(VARCHAR(10), dateadd(d, - 2, @RecordTime), 111)
	SET @ToDate = convert(VARCHAR(10), @RecordTime, 111)

	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
		,@FromDate
		,@ToDate
		,0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecord_UpdateType]
    @RecordTimeId INT
	,@EmployeeId INT
    ,@InOutType TINYINT
    ,@ModifiedBy INT
    ,@ModifiedDate DATETIME
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140724@Quinn.Vo
Task: Update Time Record.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
    = 0: Transaction recorded successfully.
    > 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @EmpId INT, @FromDate DATETIME, @ToDate DATETIME

    SELECT @EmpId = EmpID, @FromDate = CONVERT(VARCHAR(10), DATEADD(d, -1, RecordTime), 111), @ToDate = CONVERT(VARCHAR(10), RecordTime, 111)
    FROM dbo.TimeRecord WITH (NOLOCK)
    WHERE ID = @RecordTimeId

	IF(@InOutType = 2)
		BEGIN
			UPDATE dbo.TimeRecord WITH (ROWLOCK, UPDLOCK)
			SET InOutType = NULL,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = dbo.fnc_GetClientDateTime()
			WHERE ID = @RecordTimeId
		END
	ELSE
		BEGIN
			UPDATE dbo.TimeRecord WITH (ROWLOCK, UPDLOCK)
			SET InOutType = @InOutType,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = dbo.fnc_GetClientDateTime()
			WHERE ID = @RecordTimeId
		END

    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @FromDate, @ToDate, 1

    IF @@ERROR <> 0
    BEGIN
        SELECT @ErrCode = 1
            ,@ErrMsg = N'Update Time Record failed.'
    END
    ELSE
    BEGIN
        SELECT @ErrCode = 0
            ,@ErrMsg = ''
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TimeRecordEXT_UpdateStatus] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@Status TINYINT
AS
/*
Created: 20211229@Teddy.le
Task: Update status time record
DB: HRM
Original: 

Revisions:  

*/
BEGIN
	SET NOCOUNT ON;
	IF EXISTS (
			SELECT 1
			FROM [dbo].[TimeRecord] AS tr WITH (NOLOCK)
				 INNER JOIN [dbo].[TimeRecordExt] AS tre WITH (NOLOCK) ON tr.Id = tre.ID
			WHERE tr.EmpId = @EmpId
				AND convert(VARCHAR(10), tr.RecordTime, 111) BETWEEN @FromDate AND @ToDate
			)
	BEGIN
		UPDATE tre WITH (ROWLOCK, UPDLOCK)
		SET Status = @Status
		FROM [dbo].[TimeRecordExt] AS tre WITH  (ROWLOCK, UPDLOCK) 
				 INNER JOIN  [dbo].[TimeRecord] AS tr WITH  (ROWLOCK, UPDLOCK) ON tr.Id = tre.ID
		WHERE tr.EmpId = @EmpId
			AND convert(VARCHAR(10), tr.RecordTime, 111)  BETWEEN @FromDate AND @ToDate
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TOEIC_GetAllScores] @query NVARCHAR(256) = NULL
	,@groupId INT = NULL
	,@pageNumber INT = 1
	,@pageSize INT = 50
	,@totalRow INT = 0 OUTPUT
AS
/*
Created: 
Task: get employee's TOEIC score
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @q NVARCHAR(256)

	SET @q = '%' + @query + '%'

	IF (LEN(@query) = 0)
	BEGIN
		SET @query = NULL
	END

	SELECT @totalRow = COUNT(1)
	FROM dbo.Employee WITH (NOLOCK)
	WHERE IsSpecialUser = 0
		AND (
			@query IS NULL
			OR NickName LIKE @q
			)
		AND (
			@groupId IS NULL
			OR GroupId = @groupId
			)

	-- Insert statements for procedure here
	SELECT e.Id
		,e.NickName
		,e.FullName
		,e.JoinedDate
		,g.ID AS [TeamID]
		,g.Code
		,g.[Name]
		,g.[Rank]
		,jt.JobTitleId
		,jt.[Name]
		,ISNULL(LRExamDate, '1900-01-01') AS LRExamDate
		,ISNULL(t.Listening, 0) AS Listening
		,ISNULL(t.Reading, 0) AS Reading
		,ISNULL(t.Writing, 0) AS Writing
		,ISNULL(t.Speaking, 0) AS Speaking
		,t.InputDate
		,SWExamDate
		,ISNULL(t.OldListening, 0) AS OldListening
		,ISNULL(t.OldReading, 0) AS OldReading
		,ISNULL(t.OldWriting, 0) AS OldWriting
		,ISNULL(t.OldSpeaking, 0) AS OldSpeaking
		,ISNULL(t.MinimumScore, 0) AS MinimumScore
	FROM (
		SELECT *
		FROM dbo.Employee WITH (NOLOCK)
		WHERE IsSpecialUser = 0
			AND ISNULL(Enabled, 1) = 1
			AND ISNULL(isDeleted, 0) = 0
			AND (
				@query IS NULL
				OR NickName LIKE @q
				)
			AND (
				@groupId IS NULL
				OR GroupId = @groupId
				)
		) AS e
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupID = g.ID
		LEFT JOIN HRM.dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		LEFT JOIN HRM.dbo.TOEIC AS t WITH (NOLOCK) ON e.Id = t.EmpId
	ORDER BY [Rank]
		,g.Code
		,e.Roles DESC
		,GroupDate
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TOEIC_GetScore] @empId INT
AS
/*
Created: 
Task: get employee's TOEIC score
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT TOP 1 EmpId
		,LRExamDate
		,Listening
		,OldListening
		,Reading
		,OldReading
		,SWExamDate
		,Speaking
		,OldSpeaking
		,Writing
		,OldWriting
		,InputDate
		,MinimumScore
	FROM HRM.dbo.TOEIC WITH(NOLOCK)
	WHERE EmpId = @empId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TOEIC_GetUpdatingScoreRelatedPeople] @id INT
AS
/*
    Created: 20150403@Tim.Trinh
    Task: Get related people to send mail when update toeic.
    DB: NexEPA
    Original: 

    Revisions:    
        - 20250327@Henry: Update MGT Code -> MGT/COR code
    Param's Explanation:
*/
BEGIN
    SELECT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.Email
    FROM [dbo].[Employee] e WITH (NOLOCK)
    WHERE e.ID = @id
    
    UNION
    SELECT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.Email
    FROM [dbo].[Employee] e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeGroup AS gr WITH (NOLOCK) ON e.GroupId = gr.Id
    WHERE gr.Code IN ('COR', 'MGT')
	AND e.isDeleted = 0
    
    UNION
    SELECT DISTINCT e.ID
        ,e.NickName
        ,e.FullName
        ,e.roles
        ,e.Email
    FROM dbo.Employee e WITH (NOLOCK)
        INNER JOIN dbo.EmployeeSupDetail AS sup WITH (NOLOCK) ON e.Id = sup.ApproverId
    WHERE e.isDeleted = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TOEIC_Insert] @empId INT
	,@listening INT = NULL
	,@reading INT = NULL
	,@lrExamDate DATETIME = NULL
	,@oldListening INT = NULL
	,@oldReading INT = NULL
	,@speaking INT = NULL
	,@writing INT = NULL
	,@swExamDate DATETIME = NULL
	,@oldSpeaking INT = NULL
	,@oldWriting INT = NULL
	,@MinimumScore INT = NULL
	,@inputDate DATETIME = NULL
	,@modifiedBy INT = NULL
	,@modifiedDate DATETIME = NULL
	,@createdBy INT = NULL
	,@createdDate DATETIME = NULL
AS
/*
Created: 
Task: create new employee's TOEIC score
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO HRM.dbo.TOEIC (
		EmpId
		,Listening
		,Reading
		,LRExamDate
		,OldListening
		,OldReading
		,Speaking
		,Writing
		,SWExamDate
		,OldSpeaking
		,OldWriting
		,MinimumScore
		,InputDate
		,ModifiedBy
		,ModifiedDate
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@empId
		,@listening
		,@reading
		,@lrExamDate
		,@oldListening
		,@oldReading
		,@speaking
		,@writing
		,@swExamDate
		,@oldSpeaking
		,@oldWriting
		,@MinimumScore
		,@inputDate
		,@modifiedBy
		,@modifiedDate
		,@createdBy
		,@createdDate
		)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TOEIC_NewUpdate] @empId INT
	,@listening INT = NULL
	,@reading INT = NULL
	,@lrExamDate DATETIME = NULL
	,@speaking INT = NULL
	,@writing INT = NULL
	,@MinimumScore INT = NULL
	,@swExamDate DATETIME = NULL
	,@inputDate DATETIME = NULL
	,@modifiedBy INT = NULL
	,@modifiedDate DATETIME = NULL
	,@createdBy INT = NULL
	,@createdDate DATETIME = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 
Task: only update new TOEIC score
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM HRM.dbo.TOEIC WITH (NOLOCK)
			WHERE EmpId = @empId
			)
	BEGIN -- update
		IF (
				(@listening + @reading) > 0
				AND (@speaking + @writing) > 0
				) -- LR & SW
		BEGIN
			UPDATE HRM.dbo.TOEIC WITH (UPDLOCK, ROWLOCK)
			SET Listening = @listening
				,Reading = @reading
				,LRExamDate = @lrExamDate
				,Speaking = @speaking
				,Writing = @writing
				,MinimumScore = @MinimumScore
				,SWExamDate = @swExamDate
				,InputDate = @inputDate
				,ModifiedBy = @modifiedBy
				,ModifiedDate = @modifiedDate
			WHERE EmpId = @empId
		END

		IF (@listening + @reading > 0) -- LR
		BEGIN
			UPDATE HRM.dbo.TOEIC WITH (UPDLOCK, ROWLOCK)
			SET Listening = @listening
				,Reading = @reading
				,MinimumScore = @MinimumScore
				,LRExamDate = @lrExamDate
				,InputDate = @inputDate
				,ModifiedBy = @modifiedBy
				,ModifiedDate = @modifiedDate
			WHERE EmpId = @empId
		END
		ELSE IF (@speaking + @writing > 0) -- SW
		BEGIN
			UPDATE HRM.dbo.TOEIC WITH (UPDLOCK, ROWLOCK)
			SET Speaking = @speaking
				,Writing = @writing
				,MinimumScore = @MinimumScore
				,SWExamDate = @swExamDate
				,InputDate = @inputDate
				,ModifiedBy = @modifiedBy
				,ModifiedDate = @modifiedDate
			WHERE EmpId = @empId
		END
	END
	ELSE
	BEGIN -- insert
		EXEC dbo.TOEIC_Insert @empId
			,@listening
			,@reading
			,@lrExamDate
			,NULL
			,NULL
			,@speaking
			,@writing
			,@swExamDate
			,NULL
			,NULL
			,@MinimumScore
			,@inputDate
			,@modifiedBy
			,@modifiedDate
			,@createdBy
			,@createdDate
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TOEIC_Update] @empId INT
	,@listening INT = NULL
	,@reading INT = NULL
	,@lrExamDate DATETIME = NULL
	,@oldListening INT = NULL
	,@oldReading INT = NULL
	,@speaking INT = NULL
	,@writing INT = NULL
	,@MinimumScore INT = NULL
	,@swExamDate DATETIME = NULL
	,@oldSpeaking INT = NULL
	,@oldWriting INT = NULL
	,@inputDate DATETIME = NULL
	,@oldDeadlineDate DATETIME = NULL
	,@modifiedBy INT = NULL
	,@modifiedDate DATETIME = NULL
	,@createdBy INT = NULL
	,@createdDate DATETIME = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 
Task: update employee's TOEIC score
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

DECLARE @monthEvaluation1H TINYINT = 5,
@monthEvaluation2H TINYINT = 11,
@toDay DATETIME = dbo.fnc_GetClientDateTime()

BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM HRM.dbo.TOEIC WITH (NOLOCK)
			WHERE EmpId = @empId
			AND ( Listening + Reading ) >= @MinimumScore
			AND InputDate > @toDay
			AND (MONTH(InputDate) != @monthEvaluation1H OR MONTH(InputDate) != @monthEvaluation2H)
			AND YEAR(InputDate) != YEAR(@toDay)
			AND MONTH(@toDay) IN ( @monthEvaluation1H, @monthEvaluation2H)
			AND (MONTH(@lrExamDate) = @monthEvaluation1H OR MONTH(@lrExamDate) = @monthEvaluation2H)
			AND YEAR(@lrExamDate) = YEAR(@toDay)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Toeic score still effect. Please do not update scores this time.'

			RETURN
	END;
	ELSE
	BEGIN
		IF EXISTS (
			SELECT 1
			FROM HRM.dbo.TOEIC WITH (NOLOCK)
			WHERE EmpId = @empId
			)
		BEGIN -- update
		UPDATE HRM.dbo.TOEIC WITH (UPDLOCK, ROWLOCK)
		SET Listening = @listening
			,Reading = @reading
			,LRExamDate = @lrExamDate
			,OldListening = @oldListening
			,OldReading = @oldReading
			,Speaking = @speaking
			,Writing = @writing
			,SWExamDate = @swExamDate
			,OldSpeaking = @oldSpeaking
			,OldWriting = @oldWriting
			,MinimumScore = @MinimumScore
			,InputDate = @inputDate
			,ModifiedBy = @modifiedBy
			,ModifiedDate = @modifiedDate
		WHERE EmpId = @empId;
	END;
	ELSE
	BEGIN -- insert
		INSERT INTO HRM.dbo.TOEIC (
		EmpId
		,Listening
		,Reading
		,LRExamDate
		,OldListening
		,OldReading
		,Speaking
		,Writing
		,SWExamDate
		,OldSpeaking
		,OldWriting
		,MinimumScore
		,InputDate
		,ModifiedBy
		,ModifiedDate
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@empId
		,@listening
		,@reading
		,@lrExamDate
		,@oldListening
		,@oldReading
		,@speaking
		,@writing
		,@swExamDate
		,@oldSpeaking
		,@oldWriting
		,@MinimumScore
		,@inputDate
		,@modifiedBy
		,@modifiedDate
		,@createdBy
		,@createdDate
		)
	END;
	END;

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Toeic score failed.';
	END;
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = '';
	END;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopment_Get]
     @GroupId INT = NULL
    ,@EmpId INT = NULL
    ,@CurrentUserId INT
    ,@TypeId INT = NULL
    ,@Name NVARCHAR(200) = NULL
    ,@FromDate DATETIME = NULL
    ,@ToDate DATETIME = NULL
AS
/*
Created: 20230417@Brian.Nguyen
Task: Get Training And Development
DB: NexHR
Original:
Revisions:
- 20241202@Lilac: Add certificate upload
- 20250520@Henry: Add default value for FromDate and ToDate
Param's Explanation:
*/
BEGIN
    SET NOCOUNT ON;
    DECLARE
            @UserId INT
            ,@UserRoles NVARCHAR(20)
            ,@UserGroupId INT
    SELECT @UserId = Id, @UserRoles = Roles, @UserGroupId = GroupId
    FROM dbo.Employee WITH (NOLOCK)
    WHERE Id = @CurrentUserId
    SELECT
        cc.Id,
        cc.EmpId,
        e.NickName,
        e.FullName,
        e.GroupId,
        cc.ConductedBy,
        cc.Description,
        eg.Name AS TeamName,
        eg.Code AS TeamCode,
        cc.TypeId,
        tt.Name AS TypeName,
        cc.Name,
        cc.StartDate,
        cc.EndDate,
        cc.ExamDate,
        cc.ExpirationDate,
        cc.Organization,
        cc.Publish,
        cc.Certificate
    FROM HRM.[dbo].[TrainingAndDevelopment] AS cc WITH (NOLOCK)
        INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK)
            ON cc.EmpId = e.Id
        INNER JOIN [dbo].[EmployeeGroup] AS eg WITH (NOLOCK)
            ON e.GroupId = eg.Id
        INNER JOIN HRM.[dbo].[TrainingAndDevelopmentType] AS tt WITH (NOLOCK)
            ON cc.TypeId = tt.Id
    WHERE  e.Enabled = 1
        AND e.isDeleted = 0
        AND e.IsSpecialUser = 0
        AND (@GroupId IS NULL
                OR e.GroupId = @GroupId
            )
            AND (
                @EmpId IS NULL
                OR e.Id = @EmpId
            )
            AND (
                @TypeId IS NULL
                OR @TypeId = cc.TypeId
            )
            AND (
                @Name IS NULL
                OR cc.Name Like ('%' + @Name + '%')
            )
            AND (
                @FromDate IS NULL
                OR cc.CreatedDate >= @FromDate
            )
            AND (
                @ToDate IS NULL
                OR cc.CreatedDate <= @ToDate
            )
            AND(
                @UserRoles IN ('3','4')
                OR cc.Publish LIKE 'Public'
                OR (cc.Publish LIKE 'Protected' AND
                (cc.EmpId = @UserId
                    OR e.GroupId = @UserGroupId
                    OR EXISTS(SELECT 1 FROM dbo.EmployeeSupDetail WITH (NOLOCK)
                    WHERE EmpId = cc.EmpId AND ApproverId = @UserId)
                ))
                OR (cc.Publish LIKE 'Private' AND cc.EmpId = @UserId)
            )
    ORDER BY cc.CreatedDate DESC
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopment_GetByEmpId]
	 @EmpId INT
	,@CurrentUserId INT
AS
/*
Created: 20230418@Brian.Nguyen
Task: Get Training And Development by employee id
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE 
			@UserId INT
			,@UserRoles NVARCHAR(20)
			,@UserGroupId INT

	SELECT @UserId = Id, @UserRoles = Roles, @UserGroupId = GroupId
	FROM dbo.Employee WITH (NOLOCK) 
	WHERE Id = @CurrentUserId

	SELECT 
		cc.Id,
		cc.EmpId,
		e.NickName, 
		e.FullName,
		e.GroupId,
		cc.ConductedBy, 
		cc.Description,
		eg.Name AS TeamName,
		eg.Code AS TeamCode,
		cc.TypeId, 
		tt.Name AS TypeName, 
		cc.Name, 
		cc.StartDate, 
		cc.EndDate, 
		cc.ExamDate, 
		cc.ExpirationDate, 
		cc.Organization,
		cc.Publish
	FROM HRM.[dbo].[TrainingAndDevelopment] AS cc WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK)
			ON cc.EmpId = e.Id
		INNER JOIN [dbo].[EmployeeGroup] AS eg WITH (NOLOCK)
			ON e.GroupId = eg.Id
		INNER JOIN HRM.[dbo].[TrainingAndDevelopmentType] AS tt WITH (NOLOCK)
			ON cc.TypeId = tt.Id
	WHERE  @EmpId = cc.EmpId
			AND(
				@UserRoles IN ('3','4')
				OR cc.Publish LIKE 'Public'
				OR (cc.Publish LIKE 'Protected' AND 
				(cc.EmpId = @UserId 
					OR e.GroupId = @UserGroupId 
					OR EXISTS(SELECT 1 FROM dbo.EmployeeSupDetail WITH (NOLOCK)
					WHERE EmpId = cc.EmpId AND ApproverId = @UserId) 
				))
				OR (cc.Publish LIKE 'Private' AND cc.EmpId = @UserId )
			)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TrainingAndDevelopment_GetById]
	 @Id INT
AS
/*
Created: 20230418@Brian.Nguyen
Task: get Training And Development by id
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
		cc.Id,
		cc.EmpId,
		e.NickName, 
		e.FullName,
		e.GroupId,
		cc.ConductedBy, 
		cc.Description,
		eg.Name AS TeamName,
		eg.Code AS TeamCode,
		cc.TypeId, 
		tt.Name AS TypeName,
		cc.Name, 
		cc.StartDate, 
		cc.EndDate, 
		cc.ExamDate, 
		cc.ExpirationDate, 
		cc.Organization,
		cc.Publish,
		cc.[Certificate]
	FROM HRM.[dbo].[TrainingAndDevelopment] AS cc WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK)
			ON cc.EmpId = e.Id
		INNER JOIN [dbo].[EmployeeGroup] AS eg WITH (NOLOCK)
			ON e.GroupId = eg.Id
		INNER JOIN HRM.[dbo].[TrainingAndDevelopmentType] AS tt WITH (NOLOCK)
			ON cc.TypeId = tt.Id
	WHERE  @Id = cc.Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_Delete] @WorkFromHomeRequestId INT
	,@DeletedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211224@Teddy.le
Task: Delete Work From Home Request.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @empId INT, @fromDate DATE, @toDate DATE
	SELECT @empId = wfhr.EmpId , @fromDate = wfhr.FromDate, @toDate = wfhr.ToDate
			FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE Id = @WorkFromHomeRequestId

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE Id = @WorkFromHomeRequestId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH does not exists'

		RETURN
	END

	DELETE
	FROM dbo.WorkFromHomeRequest 
	WHERE Id = @WorkFromHomeRequestId

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 0

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @fromDate, @toDate, 3

	SELECT @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_DisApproveRequest]
    @WorkFromHomeRequestId INT,
    @ProcessedBy INT,
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211228@Teddy.le
Task: Disapprove request employee work from home
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN

	DECLARE @EmpId INT, @Status TINYINT, @FromDate DATETIME, @ToDate DATETIME, @CurrentTime DATETIME
	SELECT @CurrentTime =  dbo.fnc_GetClientDateTime()

	SELECT @EmpId = wfhr.EmpId, @Status = wfhr.Status, 
			@FromDate = wfhr.FromDate, @ToDate = wfhr.ToDate
	FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
	WHERE wfhr.Id = @WorkFromHomeRequestId

	SET @FromDate = CONVERT(VARCHAR(10),@FromDate,111) 
	set @ToDate = CONVERT(VARCHAR(10),@ToDate,111) 

	IF (@Status=3)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'This request was rejected so it could not be rejected again'

		RETURN
	END

	-- if status is pending
	IF (@Status=1)
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = 'This request is pending so it cannot be disapproved'
    
		RETURN
	END

	-- if not exist calendar

	IF NOT EXISTS(SELECT 1
					FROM dbo.EmployeeCalendar WITH (NOLOCK)
					WHERE empid=@empid
						AND workdate = @fromdate
						)
	BEGIN
		SET @ErrCode = 102
		SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'
		RETURN
	END

	UPDATE dbo.WorkFromHomeRequest WITH (UPDLOCK, ROWLOCK)
	SET [Status] = 1,
		ProcessedBy= @ProcessedBy,
		ProcessedDate= @CurrentTime,
		ModifiedDate = @CurrentTime,
		ModifiedBy = @ProcessedBy
	WHERE Id = @WorkFromHomeRequestId


	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, 1

	-- write log
	Declare @LogSPData varchar(4000), @now datetime
	set @now=dbo.fnc_GetClientDateTime()

	--Log general info
	Select @LogSPData = '<EmployeeWorkFromHome_DisApproveRequest @WorkFromHomeRequestId="'+ coalesce(cast(@WorkFromHomeRequestId as varchar(50)),space(1)) + '" ' +
	'FromDate="'+ coalesce(cast(@FromDate as varchar(100)),space(1)) + '" ' +
	'ToDate="'+ coalesce(cast(@ToDate as varchar(100)),space(1)) + '" ' +
	'ProcessedBy="'+ coalesce(cast(@ProcessedBy as varchar(100)),space(1)) + '" ' +
	'ProcessedDate="'+ coalesce(convert(varchar(10),@CurrentTime, 111 ),space(1)) + '" ' +
	'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
	'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'    

	EXEC dbo.Log_WriteLog
		@LogActionID = 1,
		@LogDate = @now,
		@Description = 'DisApprove Employee Work From Home Request',
		@TrackingValue = @LogSPData,
		@UserID = @ProcessedBy

	SET @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_Get] 
	 @FromDate DATETIME
	,@ToDate DATETIME
	,@EmpId INT = NULL
    ,@GroupId INT = NULL
	,@StatusId INT = NULL
AS
/*
Created: 20211208@Teddy.le
Task: Get Employee Work From Home by date.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT wfhr.Id 
		,wfhr.FromDate
		,wfhr.ToDate
		,wfhr.EmpId
		,e.NickName
		,e.FullName
		,wfhr.HRRemark
		,wfhr.Remark
		,wfhr.Status AS StatusId
		,lk.Name AS [Status]
		,wfhr.CreatedBy as CreatedById
		,creator.NickName as CreatedBy
		,wfhr.CreatedDate
		,Processor.NickName AS ProcessedBy
		,wfhr.ProcessedDate
	FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON wfhr.EmpId = e.Id
		INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON wfhr.CreatedBy = creator.Id
		LEFT JOIN dbo.Employee AS Processor WITH (NOLOCK) ON wfhr.ProcessedBy = Processor.Id
		LEFT JOIN HRM.dbo.LookupData AS lk WITH (NOLOCK) ON wfhr.Status = lk.Value AND lk.LookupId = 4
	WHERE ((wfhr.FromDate BETWEEN @FromDate AND @ToDate)
		OR (wfhr.ToDate BETWEEN @FromDate AND @ToDate)
		OR (@FromDate BETWEEN wfhr.FromDate AND wfhr.ToDate)
		OR (@ToDate BETWEEN wfhr.FromDate AND wfhr.ToDate)) 
		AND (
            @GroupId IS NULL
            OR e.GroupId = @Groupid
            )
		AND (
            @EmpId IS NULL
            OR e.Id = @EmpId
            )
		AND (
            @StatusId IS NULL
            OR wfhr.Status = @StatusId
            )
	ORDER BY wfhr.CreatedDate DESC
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_GetById] 
	@Id INT
AS
/*
Created: 20211223@Teddy.le
Task: Get Work From Home Request By Id.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/

BEGIN
	SET NOCOUNT ON;

	SELECT wfhr.Id
		,wfhr.FromDate
		,wfhr.ToDate
		,wfhr.Remark
		,wfhr.HRRemark
		,wfhr.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON wfhr.EmpId = e.Id
	WHERE wfhr.Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_Insert] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@CreatedBy INT
	,@Remark NVARCHAR(2000) = ''
	,@HRRemark NVARCHAR(2000) = ''
	,@StatusId TINYINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211223@Teddy.le
Task: Insert Work From Home Request.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ProcessedBy INT
		,@ProcessedDate DATETIME
		,@CurrentTime DATETIME

	SELECT @CurrentTime = dbo.fnc_GetClientDateTime()

	IF EXISTS (
			SELECT 1
			FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE wfhr.EmpId = @EmpId
				AND (
					@FromDate BETWEEN wfhr.FromDate
						AND wfhr.ToDate
					OR @ToDate BETWEEN wfhr.FromDate
						AND wfhr.ToDate
					)
				AND wfhr.Status <> 3
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH is overlapping.'

		RETURN
	END

	IF(@StatusId = 2)
	BEGIN
		SELECT @ProcessedDate = @CurrentTime, @ProcessedBy = @CreatedBy
	END

	INSERT INTO dbo.WorkFromHomeRequest(
		EmpId,
		Status,
		Remark,
		HRRemark,
		FromDate,
		ToDate,
		CreatedDate,
		CreatedBy,
		ProcessedBy,
		ProcessedDate
	)
	VALUES (
		@EmpId
	   ,@StatusId
	   ,@Remark
	   ,@HRRemark
	   ,@FromDate
	   ,@ToDate
	   ,dbo.fnc_GetClientDateTime()
	   ,@CreatedBy
	   ,@ProcessedBy
	   ,@ProcessedDate
		)

	
	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, @StatusId

	BEGIN
		EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate, @StatusId 
	END	
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_ProcessRequest]
    @WorkFromHomeRequestId INT,    
	@StatusId TINYINT,    
    @ProcessedBy INT,
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211227@Teddy.le
Task: Approve request employee work from home
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN

	DECLARE @EmpId INT, @Status TINYINT, @FromDate DATETIME, @ToDate DATETIME, @CurrentTime DATETIME
	SELECT @CurrentTime =  dbo.fnc_GetClientDateTime()

	SELECT @EmpId = wfhr.EmpId, @Status = wfhr.Status, 
			@FromDate = wfhr.FromDate, @ToDate = wfhr.ToDate
	FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
	WHERE wfhr.Id = @WorkFromHomeRequestId

	SET @FromDate = CONVERT(VARCHAR(10),@FromDate,111) 
	set @ToDate = CONVERT(VARCHAR(10),@ToDate,111) 

	IF (@Status<>1)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'This request is not pending so it cannot be approved or be rejected'

		RETURN
	END

	IF NOT EXISTS(SELECT 1
					FROM dbo.EmployeeCalendar WITH (NOLOCK)
					WHERE empid=@empid
						AND workdate = @fromdate                    
						)
	BEGIN
		SET @ErrCode = 102
		SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'
		RETURN
	END

	UPDATE dbo.WorkFromHomeRequest WITH (UPDLOCK, ROWLOCK)
	SET [Status] = @StatusId,
		ProcessedBy= @ProcessedBy,
		ProcessedDate= @CurrentTime,
		ModifiedDate = @CurrentTime,
		ModifiedBy = @ProcessedBy
	WHERE Id = @WorkFromHomeRequestId

	--reject
	IF (@StatusId = 3)
	BEGIN
		EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 0
	END

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, @StatusId

	-- write log
	Declare @LogSPData varchar(4000), @now datetime
	set @now=dbo.fnc_GetClientDateTime()

	--Log general info
	Select @LogSPData = '<EmployeeWorkFromHome_ApproveAndRejectRequest @WorkFromHomeRequestId="'+ coalesce(cast(@WorkFromHomeRequestId as varchar(50)),space(1)) + '" ' +
	'StatusId="'+ coalesce(cast(@StatusId as varchar(100)),space(1)) + '" ' +
	'FromDate="'+ coalesce(cast(@FromDate as varchar(100)),space(1)) + '" ' +
	'ToDate="'+ coalesce(cast(@ToDate as varchar(100)),space(1)) + '" ' +
	'ProcessedBy="'+ coalesce(cast(@ProcessedBy as varchar(100)),space(1)) + '" ' +
	'ProcessedDate="'+ coalesce(convert(varchar(10),@CurrentTime, 111 ),space(1)) + '" ' +
	'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
	'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'    

	EXEC dbo.Log_WriteLog
		@LogActionID = 1,
		@LogDate = @now,
		@Description = 'Approve And Reject Employee Work From Home Request',
		@TrackingValue = @LogSPData,
		@UserID = @ProcessedBy

	SET @ErrCode = 0
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_Update] @WorkFromHomeRequestId INT
	,@EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@Remark NVARCHAR(500) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211213@Teddy.le
Task: Update Work From Home Request.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @FromDateWFH DATE,  @ToDateWFH DATE
	SELECT @FromDateWFH = FromDate, @ToDateWFH = ToDate
			FROM dbo.WorkFromHomeRequest WITH (NOLOCK)
			WHERE Id = @WorkFromHomeRequestId
	IF EXISTS (
			SELECT 1
			FROM  dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE wfhr.Id <> @WorkFromHomeRequestId
				AND wfhr.EmpId = @EmpId
				AND (
					@FromDate BETWEEN wfhr.FromDate
						AND wfhr.ToDate
					OR @ToDate BETWEEN wfhr.FromDate
						AND wfhr.ToDate
					)
				AND wfhr.Status <> 3
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH is overlapping.'

		RETURN
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE Id = @WorkFromHomeRequestId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH does not exists'

		RETURN
	END

	UPDATE dbo.WorkFromHomeRequest WITH (ROWLOCK, UPDLOCK)
	SET  FromDate = @FromDate
		,ToDate = @ToDate
		,Remark = @Remark
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = dbo.fnc_GetClientDateTime()
	WHERE Id = @WorkFromHomeRequestId

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDateWFH, @ToDateWFH, 3

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, 1

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDateWFH ,@ToDateWFH , 0

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 1
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkFromHomeRequest_UpdateByHR] @WorkFromHomeRequestId INT
	,@EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@HRRemark NVARCHAR(500) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211213@Teddy.le
Task: Update Work From Home Request.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @FromDateWFH DATE,  @ToDateWFH DATE
	SELECT @FromDateWFH = FromDate, @ToDateWFH = ToDate
			FROM dbo.WorkFromHomeRequest WITH (NOLOCK)
			WHERE Id = @WorkFromHomeRequestId
	IF EXISTS (
			SELECT 1
			FROM  dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE wfhr.Id <> @WorkFromHomeRequestId
				AND wfhr.EmpId = @EmpId
				AND (
					@FromDate BETWEEN wfhr.FromDate
						AND wfhr.ToDate
					OR @ToDate BETWEEN wfhr.FromDate
						AND wfhr.ToDate
					)
				AND wfhr.Status <> 3
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH is overlapping.'

		RETURN
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.WorkFromHomeRequest AS wfhr WITH (NOLOCK)
			WHERE Id = @WorkFromHomeRequestId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The WFH does not exists'

		RETURN
	END

	UPDATE dbo.WorkFromHomeRequest WITH (ROWLOCK, UPDLOCK)
	SET  FromDate = @FromDate
		,ToDate = @ToDate
		,HRRemark = @HRRemark
		,ModifiedBy = @ModifiedBy
		,ProcessedBy = @ModifiedBy
		,ProcessedDate = dbo.fnc_GetClientDateTime()
		,ModifiedDate = dbo.fnc_GetClientDateTime()
	WHERE Id = @WorkFromHomeRequestId

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDateWFH, @ToDateWFH, 3

	EXEC [dbo].[TimeRecordEXT_UpdateStatus] @EmpId, @FromDate, @ToDate, 2

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDateWFH ,@ToDateWFH , 0

	EXEC [dbo].[EmployeeCalendar_UpdateWorkFromHome] @EmpId ,@FromDate ,@ToDate , 2
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest]
    @RequestID bigint,        
    @ApprovedBy int,
    @ApprovedDate datetime,
    @ErrCode int = 0 OUTPUT,
    @ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 2012/04/20@Dan
Task: approve request form (main SP)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

DECLARE @EmpId int, @RequestType smallint, @Status tinyint, @FromDate datetime, @ToDate datetime

-- get employee and requestype of the request        
SELECT @EmpId = EmpId, @RequestType = RequestType, @Status = [status], 
        @FromDate = FromDate, @ToDate = ToDate
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

set @FromDate = convert(varchar(10),@FromDate,111) 
set @ToDate = convert(varchar(10),@ToDate,111) 

-- if status is not pending, show error
IF (@Status<>1)
BEGIN
    SET @ErrCode = 100
    SET @ErrMsg = 'This request is not pending so it cannot be approved'

    RETURN
END

IF (@RequestType IN (1,2,3,4,5)
    AND EXISTS(SELECT * FROM WorkingTimeRequest 
        WHERE empid= @EmpId 
            AND RequestType=@RequestType 
            and status = 2
            and FromDate = @FromDate    
    ))
BEGIN
    SET @ErrCode = 101
    SET @ErrMsg = 'Another approved request in the same date has existed'

    RETURN
END    

-- if not exist calendar

IF (@RequestType IN (1,2,3,4,5)
    AND NOT EXISTS(SELECT *
                FROM dbo.EmployeeCalendar WITH (NOLOCK)
                WHERE empid=@empid
                    AND workdate = @fromdate                    
                    )
    )
BEGIN
    SET @ErrCode = 102
    SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'
    RETURN
END
ELSE IF (@RequestType = 6)
BEGIN    
    DECLARE    @maxDate datetime, @minDate datetime, @result bit

    SELECT @maxDate=max(workdate),@minDate = min(workdate)
    FROM dbo.EmployeeCalendar WITH (NOLOCK)
    WHERE empid=@empid
        AND workdate BETWEEN @fromdate AND @todate    

    IF (@minDate is null or @maxDate is null 
        or @fromdate <> @minDate OR @todate <> @maxDate)
    BEGIN
        SET @ErrCode = 103
        SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'

        RETURN
    END
END

-- call appropriate SP for each request form
IF (@RequestType=1)
BEGIN
    EXEC WorkingTimeRequest_ApproveRequest_1 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END
ELSE IF (@RequestType=2)
BEGIN
    EXEC WorkingTimeRequest_ApproveRequest_2 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END
ELSE IF (@RequestType=3)
BEGIN
    EXEC WorkingTimeRequest_ApproveRequest_3 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END
ELSE IF (@RequestType=4)
BEGIN
    EXEC WorkingTimeRequest_ApproveRequest_4 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END
ELSE IF (@RequestType=5)
BEGIN
    EXEC WorkingTimeRequest_ApproveRequest_5 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END
ELSE IF (@RequestType=6)
BEGIN
    EXEC WorkingTimeRequest_ApproveRequest_6 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END

-- write to history
EXEC [WorkingTimeRequest_WriteHistory] @RequestID

-- write log
Declare @LogSPData varchar(4000), @now datetime
set @now=dbo.fnc_GetClientDateTime()
--Log general info
Select @LogSPData = '<WorkingTimeRequest_ApproveRequest RequestID="'+ coalesce(cast(@RequestID as varchar(50)),space(1)) + '" ' +
'FromDate="'+ coalesce(cast(@FromDate as varchar(100)),space(1)) + '" ' +
'ToDate="'+ coalesce(cast(@ToDate as varchar(100)),space(1)) + '" ' +
'ApprovedBy="'+ coalesce(cast(@ApprovedBy as varchar(100)),space(1)) + '" ' +
'ApprovedDate="'+ coalesce(convert(varchar(10),@ApprovedDate, 111 ),space(1)) + '" ' +
'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'    

EXEC dbo.Log_WriteLog
    @LogActionID = 1,
    @LogDate = @now,
    @Description = 'Approve Request',
    @TrackingValue = @LogSPData,
    @UserID = @ApprovedBy

set @ErrCode = 0
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_1]    
    @RequestID bigint,    
    @EmpId int,
    @ApprovedBy int,
    @ApprovedDate datetime
AS
/*
Created: 2012/04/18@Dan
Task: approve check-in request form (type 1)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

DECLARE @CheckinTime datetime, @IsCrossDate BIT

-- get checkin time
select @CheckinTime = FromDate, @IsCrossDate = IsCrossDate
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- update the working calendar to record the check-in time
    UPDATE dbo.EmployeeCalendar
    SET RequestedCheckinTime = CASE WHEN @IsCrossDate = 1 THEN DATEADD(d, 1, @CheckinTime)  ELSE @CheckinTime END,
        ModifiedDate = @ApprovedDate,
        ModifiedBy = @ApprovedBy 
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @CheckinTime)=0
        
    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @CheckinTime, @CheckinTime, 1

    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 2, -- approved
        ProcessedBy= @ApprovedBy,
        ProcessedDate= @ApprovedDate,
        ModifiedDate = @ApprovedDate,
        ModifiedBy = @ApprovedBy
    WHERE Id = @RequestID
    
    COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_2] @RequestID BIGINT
	,@EmpId INT
	,@ApprovedBy INT
	,@ApprovedDate DATETIME
AS
/*
Created: 2012/04/18@Dan
Task: approve check-out request form (type 2)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
-- get request check-in time
DECLARE @CheckoutTime DATETIME, @WorkDate DATETIME, @IsCrossDate BIT

SELECT @WorkDate = FromDate,
	@IsCrossDate = IsCrossDate
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

IF (
		EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
				INNER JOIN dbo.[Shift] AS sh WITH (NOLOCK) ON cl.ShiftId = sh.Id
			WHERE sh.IsOutAfterDay = 1
			AND cl.EmpId = @EmpId
			AND DATEDIFF(dd, WorkDate, @WorkDate) = 0
			)
		)
BEGIN
	SET @CheckoutTime = DATEADD(dd, 1, @WorkDate)
END
ELSE
BEGIN
	SET @CheckoutTime = @WorkDate
END

SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	-- update the working calendar to record the check-in time
	UPDATE dbo.EmployeeCalendar
	SET RequestedCheckoutTime = CASE WHEN @IsCrossDate = 1 THEN DATEADD(d, 1, @CheckoutTime)  ELSE @CheckoutTime END
		,ModifiedDate = @ApprovedDate
		,ModifiedBy = @ApprovedBy
	WHERE EmpID = @EmpId
		AND datediff(d, WorkDate, @WorkDate) = 0

	-- process the calendar into attendance
	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
		,@CheckoutTime
		,@CheckoutTime
		,1

	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 2
		,-- approved
		ProcessedBy = @ApprovedBy
		,ProcessedDate = @ApprovedDate
		,ModifiedDate = @ApprovedDate
		,ModifiedBy = @ApprovedBy
	WHERE Id = @RequestID

	COMMIT TRANSACTION;
END TRY

BEGIN CATCH
	DECLARE @ErrorMessage NVARCHAR(4000);
	DECLARE @ErrorSeverity INT;
	DECLARE @ErrorState INT;

	SELECT @ErrorMessage = ERROR_MESSAGE()
		,@ErrorSeverity = ERROR_SEVERITY()
		,@ErrorState = ERROR_STATE();

	ROLLBACK TRANSACTION

	-- Use RAISERROR inside the CATCH block to return 
	-- error information about the original error that 
	-- caused execution to jump to the CATCH block.
	RAISERROR (
			@ErrorMessage, -- Message text.
			@ErrorSeverity, -- Severity.
			@ErrorState -- State.
			);
END CATCH;
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_3]
    @RequestID bigint,    
    @EmpId int,
    @ApprovedBy int,
    @ApprovedDate datetime
AS
/*
Created: 2012/04/18@Dan
Task: approve late in request form (type 3)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

-- get request latein dates
DECLARE @WorkDate datetime, @LateIn int

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    SELECT @WorkDate = FromDate, @LateIn = LateInMins 
    FROM dbo.WorkingTimeRequest
    WHERE Id = @RequestID

    -- update the working calendar to record the check-in time
    UPDATE dbo.EmployeeCalendar
    SET LateIn = @LateIn,
        ModifiedDate = @ApprovedDate,
        ModifiedBy = @ApprovedBy
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @WorkDate)=0

    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @WorkDate, @WorkDate, 1

    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 2 ,-- approved
        ProcessedBy= @ApprovedBy,
        ProcessedDate= @ApprovedDate,
        ModifiedDate = @ApprovedDate,
        ModifiedBy = @ApprovedBy
    WHERE Id = @RequestID
     
    COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;    
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_4]
    @RequestID bigint,    
    @EmpId int,
    @ApprovedBy int,
    @ApprovedDate datetime
AS
/*
Created: 2012/04/18@Dan
Task: approve early out request form (type 4)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
-- get request dates
DECLARE @WorkDate datetime, @EarlyOut int

SELECT @WorkDate = FromDate, @EarlyOut = EarlyOutMins
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- update the working calendar to record the check-in time
    UPDATE dbo.EmployeeCalendar
    SET EarlyOut = @EarlyOut,
        ModifiedDate = @ApprovedDate,
        ModifiedBy = @ApprovedBy
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @WorkDate)=0

    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @WorkDate, @WorkDate, 1

    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 2 ,-- approved
        ProcessedBy= @ApprovedBy,
        ProcessedDate= @ApprovedDate,
        ModifiedDate = @ApprovedDate,
        ModifiedBy = @ApprovedBy
    WHERE Id = @RequestID
     
    COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;    
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_5]
    @RequestID bigint,    
    @EmpId int,
    @ApprovedBy int,
    @ApprovedDate datetime
AS
/*
Created: 2012/04/18@Dan
Task: approve changing working shift request form (type 5)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

-- get all request detail 
DECLARE @ChangeShiftID int, @WorkDate datetime, @FromShift int, @ToShift int
    
SELECT @WorkDate = FromDate, @FromShift = FromShift,  @ToShift = ToShift, 
        @ChangeShiftID = Id
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- update the working calendar to record the check-in time
    UPDATE dbo.EmployeeCalendar
    SET  ChangeShiftID = @ChangeShiftID,
        ShiftId = @ToShift,
        ModifiedDate = @ApprovedDate,
        ModifiedBy = @ApprovedBy
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @WorkDate)=0

    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @WorkDate, @WorkDate, 1

    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 2 ,-- approved
        ProcessedBy= @ApprovedBy,
        ProcessedDate= @ApprovedDate,
        ModifiedDate = @ApprovedDate,
        ModifiedBy = @ApprovedBy
    WHERE Id = @RequestID
 
    COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;    
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_6] @RequestID BIGINT
	,@EmpId INT
	,@ApprovedBy INT
	,@ApprovedDate DATETIME
AS
/*
Created: 2012/04/19@Dan
Task: approve leave request form (type 6)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN TRY
	BEGIN TRANSACTION;

	DECLARE @LeaveTypeID INT
	,@TotalDays NUMERIC(5, 1)
	,@FromDate DATETIME
	,@ToDate DATETIME

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
		,@LeaveTypeID = LeaveTypeID
		,@TotalDays = TotalDays
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE Id = @RequestID

	-- insert leave record		
	IF (
			EXISTS (
				SELECT 1
				FROM dbo.LeaveType
				WHERE Id = @LeaveTypeID
					AND IsALDeduction = 1
				)
			)
	BEGIN
		EXEC AnnualLeave_InsertTransaction @EmpId
			,@RequestID
			,'Leave Request'
			,@FromDate
			,@ToDate
			,@TotalDays
			,NULL
			,@ApprovedBy
	END

	IF (@LeaveTypeID = 31)
	BEGIN
		EXEC [dbo].[Overtime_InsertTransactionFromLeaveRequest] @RequestID
			,@EmpId
			,@FromDate
			,@TotalDays
			,@ApprovedBy
	END

	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 2
		,-- approved
		ProcessedBy = @ApprovedBy
		,ProcessedDate = @ApprovedDate
		,ModifiedDate = @ApprovedDate
		,ModifiedBy = @ApprovedBy
	WHERE Id = @RequestID

	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
		,@FromDate
		,@ToDate
		,1

	COMMIT TRANSACTION;
END TRY

BEGIN CATCH
	DECLARE @ErrorMessage NVARCHAR(4000);
	DECLARE @ErrorSeverity INT;
	DECLARE @ErrorState INT;

	SELECT @ErrorMessage = ERROR_MESSAGE()
		,@ErrorSeverity = ERROR_SEVERITY()
		,@ErrorState = ERROR_STATE();

	ROLLBACK TRANSACTION

	-- Use RAISERROR inside the CATCH block to return 
	-- error information about the original error that 
	-- caused execution to jump to the CATCH block.
	RAISERROR (
			@ErrorMessage, -- Message text.
			@ErrorSeverity, -- Severity.
			@ErrorState -- State.
			);
END CATCH;
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_AvaibleLeaveBalance] 
	@EmpId INT
	,@EffectedDate DATETIME = NULL
	,@RequestId BIGINT = NULL
AS
/*
Created: Quinn.Vo
Task: Get  Avaible Balance Leave From Request ID
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @AnnualLeaveBalance NUMERIC(18, 2)
		,@AnnualLeaveCanBeUsed NUMERIC(18, 2)
		,@EndYear DATETIME

	IF @EffectedDate IS NULL
	BEGIN
		SELECT @EffectedDate = CONVERT(DATETIME, CONVERT(NVARCHAR(10), YEAR(dbo.fnc_GetClientDateTime())) + '/12/31')
	END

	SET @EndYear = CONVERT(DATETIME, CONVERT(NVARCHAR(10), YEAR(@EffectedDate)) + '/12/31')

	SELECT @AnnualLeaveBalance = dbo.fnc_GetAnnualLeaveBalanceByDate(@EmpId, @EndYear, @RequestID)
		,@AnnualLeaveCanBeUsed = dbo.fnc_GetAnnualLeaveBalanceByDate(@EmpId, @EffectedDate, @RequestID)

	SELECT MONTH(@EffectedDate) AS [Month]
		,YEAR(@EffectedDate) AS [Year]
		,@AnnualLeaveBalance AS AnnualLeaveBalance
		,@AnnualLeaveCanBeUsed AS AnnualLeaveCanBeUsed
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_CreateLeaveRequest_UpdateCalendar] 
	@RequestId BIGINT
	,@EmpId INT
	,@LeaveTypeId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@TotalDays NUMERIC(5, 1)
	,@ApprovedBy INT
AS
/*
Created: Quinn.Vo
Task: Update Calendar when approve leave request
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @ExistLeaveTypeId INT

	SELECT @ExistLeaveTypeId = LeaveTypeId
	FROM dbo.EmployeeCalendar
	WHERE EmpID = @EmpId
		AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
			AND CONVERT(VARCHAR(10), @ToDate, 111)

	IF (@ExistLeaveTypeId IS NULL)
	BEGIN
		UPDATE dbo.EmployeeCalendar
		SET IsLeaveDay = 1
			,LeaveTypeID = @LeaveTypeID
			,LeaveDay = CASE 
				WHEN @TotalDays = 0.5 THEN @TotalDays
				ELSE 1
				END
			,ModifiedDate = dbo.fnc_GetClientDateTime()
			,ModifiedBy = @ApprovedBy
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
				AND CONVERT(VARCHAR(10), @ToDate, 111)
	END
	ELSE
	BEGIN
		UPDATE dbo.EmployeeCalendar
		SET LeaveTypeId2 = @LeaveTypeID
			,LeaveDay = 1
			,ModifiedDate = dbo.fnc_GetClientDateTime()
			,ModifiedBy = @ApprovedBy
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
				AND CONVERT(VARCHAR(10), @ToDate, 111)
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_Delete] 
    @id int
    ,@ErrCode INT = 0 OUTPUT
    ,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@LeaveTypeId INT
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@TotalDays NUMERIC(5, 1)
		,@RequestType SMALLINT
		,@Status INT

	SELECT 
		@EmpId = EmpId
		,@FromDate = FromDate
		,@ToDate = ToDate
		,@LeaveTypeID = LeaveTypeID
		,@TotalDays = TotalDays
		,@RequestType = RequestType
		,@Status = Status
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE Id = @id

	IF (@Status = 1)
	BEGIN
		DECLARE @LogSPData VARCHAR(4000), @now DATETIME
		SET @now = dbo.fnc_GetClientDateTime()
		SELECT @LogSPData = '<WorkingTimeRequest_Delete RequestID="'+ coalesce(cast(@id as varchar(50)),space(1)) + '" />'

		EXEC dbo.Log_WriteLog
			@LogActionID = 11,
			@LogDate = @now,
			@Description = 'Delete Request',
			@TrackingValue = @LogSPData,
			@UserID = @EmpId

		IF (@RequestType = 6)
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_DeleteLeaveRequest_UpdateCalendar] @id
			,@EmpId
			,@LeaveTypeId
			,@FromDate
			,@ToDate
			,@TotalDays
			,NULL

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
		END

		-- Insert statements for procedure here
		DELETE WorkingTimeRequest
		WHERE Id = @id
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 102, @ErrMsg = 'Status of request has been changed. Please check again.'
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DeleteLeaveRequest_UpdateCalendar] 
	@RequestId BIGINT
	,@EmpId INT
	,@LeaveTypeId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@TotalDays NUMERIC(5, 1)
	,@ModifiedBy INT = NULL
AS
/*
Created: Quinn.Vo
Task: Update Calendar when approve leave request
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @ExistLeaveTypeId INT
		,@ExistLeaveTypeId2 INT

	SELECT @ExistLeaveTypeId = LeaveTypeId
		,@ExistLeaveTypeId2 = LeaveTypeId2
	FROM dbo.EmployeeCalendar
	WHERE EmpID = @EmpId
		AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
			AND CONVERT(VARCHAR(10), @ToDate, 111)

	IF (@ExistLeaveTypeId2 IS NULL)
	BEGIN
		-- update the working calendar to clear the check-in time
		UPDATE dbo.EmployeeCalendar
		SET IsLeaveDay = 0
			,LeaveTypeID = NULL
			,LeaveDay = 0
			,ModifiedDate = dbo.fnc_GetClientDateTime()
			,ModifiedBy = @ModifiedBy
		WHERE EmpID = @EmpId
			AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
				AND CONVERT(VARCHAR(10), @ToDate, 111) 
	END
	ELSE
	BEGIN
		IF (@ExistLeaveTypeId <> @ExistLeaveTypeId2)
		BEGIN
			IF (@ExistLeaveTypeId2 = @LeaveTypeID)
			BEGIN
				UPDATE dbo.EmployeeCalendar
				SET LeaveTypeId2 = NULL
					,LeaveDay = 0.5
					,ModifiedDate = dbo.fnc_GetClientDateTime()
					,ModifiedBy = @ModifiedBy
				WHERE EmpID = @EmpId
					AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
						AND CONVERT(VARCHAR(10), @ToDate, 111)
			END

			IF (@ExistLeaveTypeId = @LeaveTypeID)
			BEGIN
				UPDATE dbo.EmployeeCalendar
				SET LeaveTypeId = @ExistLeaveTypeId2
					,LeaveTypeId2 = NULL
					,LeaveDay = 0.5
					,ModifiedDate = dbo.fnc_GetClientDateTime()
					,ModifiedBy = @ModifiedBy
				WHERE EmpID = @EmpId
					AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
						AND CONVERT(VARCHAR(10), @ToDate, 111)
			END
		END
		ELSE
		BEGIN
			UPDATE dbo.EmployeeCalendar
			SET LeaveTypeId2 = NULL
				,LeaveDay = 0.5
				,ModifiedDate = dbo.fnc_GetClientDateTime()
				,ModifiedBy = @ModifiedBy
			WHERE EmpID = @EmpId
				AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
					AND CONVERT(VARCHAR(10), @ToDate, 111)
		END
	END

	
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest]
    @RequestID bigint,        
    @ModifiedDate datetime,
    @ModifiedBy int,
    @ErrCode int = 0 OUTPUT,
    @ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 2012/04/25@Dan
Task: disapprove request form (main SP)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
DECLARE @EmpId int, @RequestType smallint, @Status tinyint

-- get employee and requestype of the request        
SELECT @EmpId = EmpId, @RequestType = RequestType, @Status = [status]
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

-- if status is rejected, show error
IF (@Status=3)
BEGIN
    SET @ErrCode = 100
    SET @ErrMsg = 'This request was rejected so it could not be rejected again'

    RETURN
END

-- if status is pending
IF (@Status=1)
BEGIN
    SET @ErrCode = 101
    SET @ErrMsg = 'This request is pending so it cannot be disapproved'
    
    RETURN
END

-- call appropriate SP for each request form
IF (@RequestType=1)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_1 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END
ELSE IF (@RequestType=2)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_2 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END
ELSE IF (@RequestType=3)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_3 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END
ELSE IF (@RequestType=4)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_4 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END
ELSE IF (@RequestType=5)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_5 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END
ELSE IF (@RequestType=6)
BEGIN
    EXEC WorkingTimeRequest_DisapproveRequest_6 @RequestID, @EmpId, @ModifiedDate, @ModifiedBy
END

-- write to history
EXEC [WorkingTimeRequest_WriteHistory] @RequestID

-- write log
Declare @LogSPData varchar(4000), @now datetime
set @now=dbo.fnc_GetClientDateTime()
--Log general info
Select @LogSPData = '<WorkingTimeRequest_DisapproveRequest RequestID="'+ coalesce(cast(@RequestID as varchar(50)),space(1)) + '" ' +
'ModifiedBy="'+ coalesce(cast(@ModifiedBy as varchar(50)),space(1)) + '" ' +
'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'    

EXEC dbo.Log_WriteLog
    @LogActionID = 3,
    @LogDate = @now,
    @Description = 'Disapprove Request',
    @TrackingValue = @LogSPData,
    @UserID = @ModifiedBy

set @ErrCode=0
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest_1]    
    @RequestID bigint,    
    @EmpId int,    
    @ModifiedDate datetime,
    @ModifiedBy int    
AS
/*
Created: 2012/04/25@Dan
Task: disApprove check-in request form (type 1)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
DECLARE @CheckinTime datetime
-- get request check-in times
select @CheckinTime = FromDate
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- update the working calendar to clear the check-in time
    UPDATE dbo.EmployeeCalendar
    SET RequestedCheckinTime = null,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @CheckinTime)=0

    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @CheckinTime, @CheckinTime, 1    

    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 1, -- pending
        ProcessedBy = @ModifiedBy ,    
        ProcessedDate = @ModifiedDate,
        ModifiedDate = @ModifiedDate,    
        ModifiedBy = @ModifiedBy 
    WHERE Id = @RequestID
    
    COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest_2]    
    @RequestID bigint,    
    @EmpId int,    
    @ModifiedDate datetime,
    @ModifiedBy int    
AS
/*
Created: 2012/04/25@Dan
Task: disapprove check-out request form (type 2)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

-- get request check-out timea
DECLARE @CheckoutTime datetime

select @CheckoutTime = FromDate
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- update the working calendar to clear the check-out time
    UPDATE dbo.EmployeeCalendar
    SET RequestedCheckoutTime = NULL,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @CheckoutTime)=0
        
    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @CheckoutTime, @CheckoutTime, 1

    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 1, -- pending
        ProcessedBy = @ModifiedBy ,    
        ProcessedDate = @ModifiedDate,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE Id = @RequestID

    COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest_3]
    @RequestID bigint,    
    @EmpId int,    
    @ModifiedDate datetime,
    @ModifiedBy int    
AS
/*
Created: 2012/04/25@Dan
Task: disapprove late in request form (type 3)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
DECLARE @RequestDetail table
        (Id int primary key identity(1,1),
        WorkDate datetime,
        LateIn int)

DECLARE @WorkDate datetime, @LateIn int
-- get late in date
SELECT @WorkDate = FromDate, @LateIn = LateInMins
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- update the working calendar to record the check-in time
    UPDATE dbo.EmployeeCalendar
    SET LateIn = 0,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @WorkDate)=0
        
    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @WorkDate, @WorkDate, 1
     
    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 1 ,-- pending
        ProcessedBy = @ModifiedBy ,    
        ProcessedDate = @ModifiedDate,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE Id = @RequestID

    COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;    
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest_4]
    @RequestID bigint,    
    @EmpId int,    
    @ModifiedDate datetime,
    @ModifiedBy int    
AS
/*
Created: 2012/04/25@Dan
Task: disapprove early out request form (type 4)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
-- get request dates
DECLARE @WorkDate datetime, @EarlyOut int

SELECT @WorkDate = FromDate, @EarlyOut = EarlyOutMins
FROM dbo.WorkingTimeRequest 
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- update the working calendar to clear the early out time
    UPDATE dbo.EmployeeCalendar
    SET EarlyOut = 0,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @WorkDate)=0

    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @WorkDate, @WorkDate, 1
        
    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 1 ,-- pending
        ProcessedBy = @ModifiedBy ,    
        ProcessedDate = @ModifiedDate,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE Id = @RequestID

    COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;    
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest_5]
    @RequestID bigint,    
    @EmpId int,    
    @ModifiedDate datetime,
    @ModifiedBy int        
AS
/*
Created: 2012/04/25@Dan
Task: disapprove changing working shift request form (type 5)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

-- get all request detail 
DECLARE @ChangeShiftID int, @WorkDate datetime, @FromShift int, @ToShift int

-- process each request detail
SELECT @WorkDate = FromDate, @FromShift = FromShift,  @ToShift = ToShift, 
        @ChangeShiftID = Id
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- update the working calendar to record the check-in time
    UPDATE dbo.EmployeeCalendar
    SET  ChangeShiftID = null,
        ShiftId = @FromShift,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE EmpID = @EmpId and datediff(d, WorkDate, @WorkDate)=0

    -- process the calendar into attendance
    EXEC EmployeeAttendance_ProcessingAndInsert @EmpId,  @WorkDate, @WorkDate, 1
    
    -- update status of request
    UPDATE dbo.WorkingTimeRequest
    SET [Status] = 1 ,-- pending
        ProcessedBy = @ModifiedBy ,    
        ProcessedDate = @ModifiedDate,
        ModifiedDate = @ModifiedDate,
        ModifiedBy = @ModifiedBy 
    WHERE Id = @RequestID

    COMMIT TRANSACTION;    
END TRY
BEGIN CATCH
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    ROLLBACK TRANSACTION

    -- Use RAISERROR inside the CATCH block to return 
    -- error information about the original error that 
    -- caused execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_DisapproveRequest_6] @RequestID BIGINT
	,@EmpId INT
	,@ModifiedDate DATETIME
	,@ModifiedBy INT
AS
/*
Created: 2012/04/25@Dan
Task: disapprove leave request form (type 6)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
DECLARE @LeaveTypeID INT
	,@TotalDays NUMERIC(5, 1)
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@LeaveTransId BIGINT

--init param
SET @LeaveTransId = 0

-- get all request detail 
SELECT @FromDate = FromDate
	,@ToDate = ToDate
	,@LeaveTypeID = LeaveTypeID
	,@TotalDays = TotalDays
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	-- get id from the leave trans
	SELECT @LeaveTransId = Id
	FROM dbo.LeaveTrans
	WHERE RequestId = @RequestId
		AND EmpID = @EmpId
		AND FromDate = @FromDate
		AND ToDate = @ToDate
		AND ISNULL(Deleted, 0) = 0

	-- delete the leave transaction
	IF (ISNULL(@LeaveTransId, 0) <> 0)
		EXEC AnnualLeave_DeleteTransaction @LeaveTransId
			,@ModifiedBy

	IF (@LeaveTypeID = 31)
	BEGIN
		EXEC [dbo].[Overtime_DeleteTransactionFromLeaveRequest] @RequestID
			,@EmpId
			,@FromDate
			,@TotalDays
			,@ModifiedBy
	END

	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
		,@FromDate
		,@ToDate
		,1

	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 1
		,-- pending
		ProcessedBy = @ModifiedBy
		,ProcessedDate = @ModifiedDate
		,ModifiedDate = @ModifiedDate
		,ModifiedBy = @ModifiedBy
	WHERE Id = @RequestID

	COMMIT TRANSACTION;
END TRY

BEGIN CATCH
	DECLARE @ErrorMessage NVARCHAR(4000);
	DECLARE @ErrorSeverity INT;
	DECLARE @ErrorState INT;

	SELECT @ErrorMessage = ERROR_MESSAGE()
		,@ErrorSeverity = ERROR_SEVERITY()
		,@ErrorState = ERROR_STATE();

	ROLLBACK TRANSACTION

	-- Use RAISERROR inside the CATCH block to return 
	-- error information about the original error that 
	-- caused execution to jump to the CATCH block.
	RAISERROR (
			@ErrorMessage, -- Message text.
			@ErrorSeverity, -- Severity.
			@ErrorState -- State.
			);
END CATCH;
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_EditLeaveRequest_UpdateCalendar] 
	@RequestId BIGINT
	,@EmpId INT
	,@LeaveTypeId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@TotalDays NUMERIC(5, 1)
	,@OldLeaveTypeId INT
	,@OldFromDate DATETIME
	,@OldToDate DATETIME
	,@OldTotalDays NUMERIC(5, 1)
	,@ModifiedBy INT
AS
/*
Created: Quinn.Vo
Task: Update Calendar when edit leave request
DB: NexHRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	EXEC [dbo].[WorkingTimeRequest_DeleteLeaveRequest_UpdateCalendar] @RequestId
		,@EmpId
		,@OldLeaveTypeId
		,@OldFromDate
		,@OldToDate
		,@OldTotalDays
		,@ModifiedBy

	EXEC [dbo].[WorkingTimeRequest_CreateLeaveRequest_UpdateCalendar] @RequestId
		,@EmpId
		,@LeaveTypeId
		,@FromDate
		,@ToDate
		,@TotalDays
		,@ModifiedBy

	IF (DATEDIFF(DD, @FromDate, @OldFromDate) >= 0 AND DATEDIFF(DD, @OldFromDate, @ToDate) >= 0)
	 OR (DATEDIFF(DD, @FromDate, @OldToDate) >= 0 AND DATEDIFF(DD, @OldToDate, @ToDate) >= 0)
	 OR (DATEDIFF(DD, @OldFromDate, @FromDate) >= 0 AND DATEDIFF(DD, @FromDate, @OldToDate) >= 0)
	 OR (DATEDIFF(DD, @OldFromDate, @ToDate) >= 0 AND DATEDIFF(DD, @ToDate, @OldToDate) >= 0)
	 BEGIN
		DECLARE @TempFromDate DATETIME
			,@TempToDate DATETIME

		IF(@FromDate > @OldFromDate)
		BEGIN
			SET @TempFromDate = @OldFromDate
		END
		ELSE
		BEGIN
			SET @TempFromDate = @FromDate
		END

		IF(@ToDate > @OldToDate)
		BEGIN
			SET @TempToDate = @ToDate
		END
		ELSE
		BEGIN
			SET @TempToDate = @OldToDate
		END

		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@TempFromDate
			,@TempToDate
			,1
	 END
	 ELSE
	 BEGIN
		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@FromDate
			,@ToDate
			,1
		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@OldFromDate
			,@OldToDate
			,1
	 END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetById] 
    @id BIGINT
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by id.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	DECLARE @IsAllowUpdateStatus TINYINT
	SET @IsAllowUpdateStatus = 1
    SELECT r.Id
        ,r.empID
        ,RequestType
        ,r.FromDate AS RequestedDate
        ,r.CreatedDate
        ,STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,ap.NickName AS ProcessedBy
        ,ap.FullName AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,ProcessedDate
        ,0.0 AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS TotalMins
        ,r.FromDate	
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,@IsAllowUpdateStatus as IsAllowUpdateStatus
		,r.IsCrossDate
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy
        JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
            AND c.WorkDate = convert(VARCHAR(10), r.FromDate, 111)
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
    WHERE r.id = @id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetNumberRequestInMonth]
    @empId INT
    ,@fromDate DATETIME
    ,@toDate DATETIME
    ,@requestType INT
AS
/*
Created: 20150730@Quinn.Vo
Task: Get numbers request in month.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
    DECLARE @NumberRequestHalfDay NUMERIC(5, 1), @NumberRequest NUMERIC(5, 1)
    IF (@requestType = 1)
    BEGIN
        SELECT @NumberRequest = COUNT(1) 
        FROM dbo.WorkingTimeRequest WITH (NOLOCK)
        WHERE EmpId = @empId
        AND RequestType IN (1, 2)
        AND [Status] IN (1, 2)
        AND DATEDIFF (D, @fromDate, FromDate) >= 0
        AND DATEDIFF (D, FromDate, @toDate) >= 0
    END
    ELSE IF (@requestType = 6)
    BEGIN
        SELECT @NumberRequestHalfDay = SUM(TotalDays)
        FROM dbo.WorkingTimeRequest WITH (NOLOCK)
        WHERE EmpId = @empId
        AND RequestType = 6
        AND [Status] IN (1, 2)
        AND TotalDays = 0.5
        AND DATEDIFF (D, @fromDate, FromDate) >= 0
        AND DATEDIFF (D, ToDate, @toDate) >= 0

        SELECT @NumberRequest = COUNT(1)
        FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
        INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) 
            ON (DATEDIFF (D, FromDate, cl.WorkDate) >= 0
            AND DATEDIFF (D, cl.WorkDate, ToDate) >= 0)
            AND wk.EmpId = cl.EmpId
        WHERE wk.EmpId = @empId
        AND RequestType = 6
        AND [Status] IN (1, 2)
        AND TotalDays <> 0.5
        AND DATEDIFF (D, @fromDate, cl.WorkDate) >= 0
        AND DATEDIFF (D, cl.WorkDate, @toDate) >= 0
        AND cl.IsOffDay <> 1
        AND cl.IsHoliday <> 1

        SELECT @NumberRequest = (ISNULL(@NumberRequestHalfDay, 0) + ISNULL(@NumberRequest, 0))
    END

    SELECT ISNULL(@NumberRequest, 0) AS NumberRequest
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetPendingRequest] 
AS
/*
Created: 20240621@Oliver.Dang
Task: Get all pending requests.
DB: NexHRM
Original: 

Revisions:    
	- 20250327@Henry: Update MGT Code -> MGT/COR code
Param's Explanation:

*/
BEGIN
	DECLARE @LeaderOfStandbyTeam INT,
		@StandByTeam VARCHAR(50)
	SELECT @LeaderOfStandbyTeam = e.Id FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON e.Id = st.Value AND st.Name = 'LeaderOfStandbyTeam'

	SELECT @StandByTeam = st.Value FROM dbo.SystemSetting AS st WITH (NOLOCK) WHERE st.Name = 'StanbyTeam'

    IF OBJECT_ID('tempdb..#PendingRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingRequest
	END

	IF OBJECT_ID('tempdb..#ManagementApprover') IS NOT NULL
	BEGIN
		DROP TABLE #ManagementApprover
	END

	CREATE TABLE #ManagementApprover(
		ApproverId INT
		,Approver NVARCHAR(20)
		,ApproverFullName NVARCHAR(50)
		,ApproverConversationId VARCHAR(200)
	)

	CREATE TABLE #PendingRequest(
		RequestId BIGINT
		,EmpId INT
		,GroupId INT
		,TeamName NVARCHAR(50)
		,Code VARCHAR(20)
		,RequestType SMALLINT
		,CreatedDate DATETIME
		,Requester NVARCHAR(20)
		,FullNameRequester NVARCHAR(50)
		,FromDate DATETIME
		,ToDate DATETIME
		,TotalDays NUMERIC(5, 1)
		,FromShift VARCHAR(10)
		,LeaveTypeId INT
		,LeaveTypeName NVARCHAR(100)
		,Remark NVARCHAR(2000)
		,IsManagementApprove BIT
		,IsStandbyAtWeekend BIT
		)

	INSERT INTO #PendingRequest
	SELECT r.Id as RequestId
		  ,r.EmpId
		  ,e.GroupId
		  ,g.Code
		  ,e.Code
		  ,r.RequestType
		  ,r.CreatedDate
  		  ,e.NickName AS Requester
		  ,e.FullName AS FullNameRequester
		  ,r.FromDate
		  ,r.ToDate
  		  ,r.TotalDays
		  ,s.Code
		  ,lv.Id AS LeaveTypeId
		  ,lv.[Name] AS LeaveTypeName
		  ,r.Remark
		  ,(CASE WHEN 
				(r.RequestType = 6) AND -- Leave type
					(r.LeaveTypeID IN (2, 30, 31) AND r.TotalDays >= 5) -- Annual Leave >= 5 days
					OR 
					r.LeaveTypeID = 26 -- Unpaid leave
			THEN 1
			ELSE 0
			END) as IsManagementApprove
			,(CASE WHEN 
				(r.RequestType IN (1,2) AND DATEPART(DW, r.FromDate) IN (1, 7) AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ',')))
			THEN 1
			ELSE 0
			END) as IsStandbyAtWeekend
	FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
	INNER JOIN dbo.Employee e with (nolock) on r.EmpId = e.Id
	INNER JOIN dbo.EmployeeGroup g with (nolock) on e.GroupId = g.Id
	LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
	LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
		AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
	LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.id = c.ShiftID
	WHERE e.[Enabled] = 1 AND ISNULL(e.isDeleted, 0 ) = 0 
		AND r.RequestType IN (1,2,6)
		AND r.[Status] = 1
	ORDER BY r.CreatedDate DESC

	IF EXISTS(SELECT 1
	FROM #PendingRequest 
	WHERE IsManagementApprove = 1)
	BEGIN
		INSERT INTO #ManagementApprover
		SELECT e.Id
			  ,e.NickName
			  ,e.FullName
			  ,con.ConversationId
		FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeGroup AS eg ON e.GroupId = eg.Id
		LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId
		WHERE e.[Enabled] = 1 AND eg.Code IN ('COR', 'MGT') AND con.ConversationId <> ''
	END

	SELECT pr.RequestId
		 ,pr.EmpId
		 ,pr.Requester
		 ,pr.FullNameRequester
		 ,pr.TeamName
		 ,pr.Code
		 ,pr.FromDate
		 ,pr.ToDate
		 ,pr.TotalDays
		 ,pr.FromShift
		 ,pr.RequestType
		 ,pr.LeaveTypeId
		 ,pr.LeaveTypeName
		 ,pr.CreatedDate
		 ,pr.Remark
		 ,ap.ApproverId
		 ,ap.Approver
		 ,ap.ApproverFullName
		 ,ap.ApproverConversationId
	FROM #PendingRequest pr
	CROSS JOIN  #ManagementApprover ap
	WHERE IsManagementApprove = 1

	UNION
	SELECT DISTINCT pr.RequestId
		 ,pr.EmpId
		 ,pr.Requester
		 ,pr.FullNameRequester
		 ,pr.TeamName
		 ,pr.Code
		 ,pr.FromDate
		 ,pr.ToDate
		 ,pr.TotalDays
		 ,pr.FromShift
		 ,pr.RequestType
		 ,pr.LeaveTypeId
		 ,pr.LeaveTypeName
		 ,pr.CreatedDate
		 ,pr.Remark
		 ,e.Id as ApproverId
		 ,e.NickName as Approver
		 ,e.FullName
		 ,con.ConversationId as ApproverConversationId
	FROM #PendingRequest pr
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.Id = @LeaderOfStandbyTeam
	LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON e.Id = con.EmpId AND con.ConversationId <> ''
	WHERE pr.IsStandbyAtWeekend = 1

	UNION

	SELECT DISTINCT pr.RequestId
		 ,pr.EmpId
		 ,pr.Requester
		 ,pr.FullNameRequester
		 ,pr.TeamName
		 ,pr.Code
		 ,pr.FromDate
		 ,pr.ToDate
		 ,pr.TotalDays
		 ,pr.FromShift
		 ,pr.RequestType
		 ,pr.LeaveTypeId
		 ,pr.LeaveTypeName
		 ,pr.CreatedDate
		 ,pr.Remark
		 ,sup.ApproverId as ApproverId
		 ,sup.ApproverNickName as Approver
		 ,sup.ApproverFullName
		 ,con.ConversationId as ApproverConversationId
	FROM #PendingRequest pr
	INNER JOIN dbo.EmployeeSupDetail AS sup WITH (NOLOCK) ON pr.EmpId = sup.EmpId
	LEFT JOIN dbo.EmployeeConversation AS con WITH (NOLOCK) ON sup.ApproverId = con.EmpId AND con.ConversationId <> ''
	WHERE pr.IsManagementApprove = 0
		AND pr.IsStandbyAtWeekend = 0

	IF OBJECT_ID('tempdb..#PendingRequest') IS NOT NULL
	BEGIN
		DROP TABLE #PendingRequest
	END

	IF OBJECT_ID('tempdb..#ManagementApprover') IS NOT NULL
	BEGIN
		DROP TABLE #ManagementApprover
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByDate] @role INT
    ,@fromdate DATETIME
    ,@todate DATETIME
    ,@empid INT = NULL
    ,@groupid INT = NULL
    ,@leaveTypeId INT = NULL
    ,@status TINYINT = NULL
	,@actorId INT = NULL
    ,@excludedEmpID INT = NULL
    ,@requestType VARCHAR(10) = ''
    ,@isFilterByEffectedDate BIT = 1
    ,@PageIndex INT = NULL
    ,@ItemsPerPage INT = NULL
    ,@TotalItem INT = NULL OUT
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by all types.
DB: NexEPA
Original: 

Revisions:    
	- 20250327@Henry: Update MGT Code -> MGT/COR code
Param's Explanation:

*/
BEGIN
    DECLARE @IsManagement BIT
        ,@IsLeaderOfStandByTeam BIT
    DECLARE @RequestTypeId TABLE (RequestType INT)
	DECLARE @LeaderOfStandbyTeam INT,
		@StandByTeam VARCHAR(50)
	SELECT @LeaderOfStandbyTeam = e.Id FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.SystemSetting AS st WITH (NOLOCK) ON e.Id = st.Value AND st.Name = 'LeaderOfStandbyTeam'

	SELECT @StandByTeam = st.Value FROM dbo.SystemSetting AS st WITH (NOLOCK) WHERE st.Name = 'StanbyTeam'


    IF OBJECT_ID('tempdb..#WorkingTimeRequest') IS NOT NULL
    BEGIN
        DROP TABLE #WorkingTimeRequest
    END

    CREATE TABLE #WorkingTimeRequest (
        Id BIGINT
        ,EmpId INT
        ,RequestType SMALLINT
        ,RequestedDate DATETIME
        ,CreatedDate DATETIME
        ,[Status] TINYINT
        ,Requester NVARCHAR(20)
        ,FullNameRequester NVARCHAR(50)
        ,EmailRequester VARCHAR(50)
        ,Team NVARCHAR(50)
        ,TeamCode VARCHAR(20)
        ,ProcessedBy NVARCHAR(20)
        ,FullNameProcessedBy NVARCHAR(50)
        ,EmailProcessedBy VARCHAR(50)
        ,ProcessedDate DATETIME
        ,TotalDaysRequestInMonth DECIMAL
        ,FromShift VARCHAR(10)
        ,ToShift VARCHAR(10)
        ,TotalMins INT
        ,FromDate DATETIME
        ,ToDate DATETIME
        ,TotalDays NUMERIC(5, 1)
        ,LeaveTypeId INT
        ,LeaveTypeName NVARCHAR(100)
        ,Remark NVARCHAR(2000)
        ,IsAllowUpdateStatus TINYINT
		,IsCrossDate BIT
        )

    IF (
            EXISTS (
                SELECT 1
                FROM [dbo].[Employee] AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeGroup AS g WITH (NOLOCK) ON e.GroupId = g.Id
                WHERE e.ID = @excludedEmpID
                    AND g.Code IN ('COR', 'MGT')
                )
            )
    BEGIN
        SET @IsManagement = 1
    END
    ELSE
    BEGIN
        SET @IsManagement = 0
    END

	IF (@excludedEmpID = @LeaderOfStandbyTeam)
	BEGIN
		SET @IsLeaderOfStandByTeam = 1
	END
	ELSE
    BEGIN
        SET @IsLeaderOfStandByTeam = 0
    END

	IF ISNULL(@requestType, '') = ''
	BEGIN
		INSERT INTO @RequestTypeId (RequestType)
		VALUES (1)
			,(2)
			,(3)
			,(4)
			,(5)
			,(6)
	END
	ELSE
	BEGIN
		INSERT INTO @RequestTypeId (RequestType)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@requestType, ',')
	END

    INSERT INTO #WorkingTimeRequest
    SELECT r.Id
        ,r.empID
        ,r.RequestType
        ,r.FromDate AS RequestedDate
        ,r.CreatedDate
        ,r.STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,ap.NickName AS ProcessedBy
        ,ap.FullName AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,r.ProcessedDate
        ,(CASE 
            WHEN r.RequestType IN (1, 2) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (1, 2)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType IN (3, 4) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (3, 4)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType = 5 THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 5
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            ELSE (
                    SELECT sum(wt.TotalDays)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 6
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    ) END
            ) AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS TotalMins
        ,r.FromDate
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,(CASE 
            WHEN (
                    @IsLeaderOfStandByTeam = 0
                    AND r.RequestType IN (1, 2)
                    AND DATEPART(DW, c.WorkDate) IN (1, 7)
					AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ','))
                    ) THEN 0
            ELSE (CASE 
                    WHEN (
                            @IsManagement = 0
                            AND r.RequestType = 6
                            AND (
                                r.LeaveTypeID = 26
                                OR (
                                    r.LeaveTypeID IN (2, 30, 31)
                                    AND r.TotalDays >= 5
                                    )
                                )
                            ) THEN 0
                    ELSE 1 END
                    ) END
            ) AS IsAllowUpdateStatus
			,r.IsCrossDate
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN dbo.EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN dbo.Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN dbo.Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy
    WHERE EXISTS (
            SELECT 1
            FROM @RequestTypeId
            WHERE [@RequestTypeId].RequestType = r.RequestType
            )
        AND (
            @empid IS NULL
            OR r.empid = @empid
            )
        AND r.STATUS <> 3
        AND (
            e.isDeleted IS NULL
            OR e.isDeleted = 0
            )
        AND (
            @excludedEmpID IS NULL
            OR r.empid <> @excludedEmpID
            )
        AND (
            @status IS NULL
            OR r.STATUS = @status
            )
		AND (@groupid IS NULL
				OR r.EmpId IN (SELECT Id FROM dbo.Employee WITH (NOLOCK) WHERE GroupId = @groupid)
			)
        AND (
                    @role NOT IN (1, 2)
                    OR r.empID IN (
						SELECT EmpId FROM dbo.EmployeeSupDetail WITH (NOLOCK) WHERE ApproverId = @excludedEmpID
                        )
            )
        AND (
            (
                @isFilterByEffectedDate = 1
                AND (
                    (
                        r.ToDate IS NULL
                        AND DATEDIFF(D, @fromdate, r.FromDate) >= 0
                        AND DATEDIFF(D, r.FromDate, @todate) >= 0
                        )
                    OR (
                        NOT (
                            DATEDIFF(D, @todate, r.FromDate) > 0
                            OR DATEDIFF(D, r.ToDate, @fromdate) > 0
                            )
                        )
                    )
                )
            OR (
                @isFilterByEffectedDate = 0
                AND (
                    DATEDIFF(D, @fromdate, r.CreatedDate) >= 0
                    AND DATEDIFF(D, r.CreatedDate, @todate) >= 0
                    )
                )
            )
        AND (
            @leaveTypeId IS NULL
            OR r.LeaveTypeID = @leaveTypeId
            )
    --ORDER BY r.FromDate DESC
    
    UNION
    SELECT r.Id
        ,r.empID
        ,r.RequestType
        ,r.FromDate AS RequestDate
        ,r.CreatedDate
        ,r.STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,ap.NickName AS ProcessedBy
        ,ap.FullName AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,r.ProcessedDate
        ,(CASE 
            WHEN r.RequestType IN (1, 2) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (1, 2)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType IN (3, 4) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (3, 4)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType = 5 THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 5
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            ELSE (
                    SELECT sum(wt.TotalDays)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 6
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    ) END
            ) AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS [Time]
        ,r.FromDate
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,1 AS IsAllowUpdateStatus
		,r.IsCrossDate
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy
    WHERE @IsManagement = 1
        AND EXISTS (
            SELECT 1
            FROM @RequestTypeId
            WHERE [@RequestTypeId].RequestType = r.RequestType
            )
        AND (
            @empid IS NULL
            OR r.empid = @empid
            )
        AND r.STATUS <> 3
        AND (
            e.isDeleted IS NULL
            OR e.isDeleted = 0
            )
        AND (
            @excludedEmpID IS NULL
            OR r.empid <> @excludedEmpID
            )
        AND (
            @status IS NULL
            OR r.STATUS = @status
            )
        AND @role IN (1, 2)
        AND (
            r.RequestType = 6
            AND (
                r.LeaveTypeID = 26
                OR (
                    r.LeaveTypeID IN (2, 30, 31)
                    AND r.TotalDays >= 5
                    )
                )
            )
        AND (@groupid IS NULL
				OR r.EmpId IN (SELECT Id FROM dbo.Employee WITH (NOLOCK) WHERE GroupId = @groupid)
			)
        AND (
            (
                @isFilterByEffectedDate = 1
                AND (
                    (
                        r.ToDate IS NULL
                        AND DATEDIFF(D, @fromdate, r.FromDate) >= 0
                        AND DATEDIFF(D, r.FromDate, @todate) >= 0
                        )
                    OR (
                        NOT (
                            DATEDIFF(D, @todate, r.FromDate) > 0
                            OR DATEDIFF(D, r.ToDate, @fromdate) > 0
                            )
                        )
                    )
                )
            OR (
                @isFilterByEffectedDate = 0
                AND (
                    DATEDIFF(D, @fromdate, r.CreatedDate) >= 0
                    AND DATEDIFF(D, r.CreatedDate, @todate) >= 0
                    )
                )
            )
        AND (
            @leaveTypeId IS NULL
            OR r.LeaveTypeID = @leaveTypeId
            )
    
    UNION
    SELECT r.Id
        ,r.empID
        ,r.RequestType
        ,r.FromDate AS RequestDate
        ,r.CreatedDate
        ,r.STATUS
        ,e.NickName AS Requester
        ,e.FullName AS FullNameRequester
        ,e.Email AS EmailRequester
        ,g.NAME AS Team
        ,g.Code AS TeamCode
        ,ap.NickName AS ProcessedBy
        ,ap.FullName AS FullNameProcessedBy
        ,ap.Email AS EmailProcessedBy
        ,r.ProcessedDate
        ,(CASE 
            WHEN r.RequestType IN (1, 2) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (1, 2)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType IN (3, 4) THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType IN (3, 4)
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            WHEN r.RequestType = 5 THEN (
                    SELECT count(1)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 5
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    )
            ELSE (
                    SELECT sum(wt.TotalDays)
                    FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
                    WHERE wt.EmpId = r.EmpId
                        AND wt.RequestType = 6
                        AND wt.STATUS = 2
                        AND wt.FromDate BETWEEN DATEADD(dd, - (DAY(r.FromDate) - 1), r.FromDate)
                            AND DATEADD(dd, - (DAY(DATEADD(mm, 1, r.FromDate)) - 1), DATEADD(mm, 1, r.FromDate))
                    ) END
            ) AS TotalDaysRequestInMonth
        ,(CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s3.Code END
            ) AS FromShift
        ,s2.Code AS ToShift
        ,CASE 
            WHEN r.RequestType = 3 THEN r.LateInMins
            ELSE r.EarlyOutMins
            END AS [Time]
        ,r.FromDate
        ,r.ToDate
        ,r.TotalDays
        ,lv.Id AS LeaveTypeId
        ,lv.NAME AS LeaveTypeName
        ,r.Remark
        ,1 AS IsAllowUpdateStatus
		,r.IsCrossDate
    FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
        INNER JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = r.empid
        AND DATEDIFF(D, c.WorkDate, r.FromDate) = 0
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id = r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id = r.ToShift
        LEFT JOIN Shift s3 WITH (NOLOCK) ON s3.id = c.ShiftID
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN Employee ap WITH (NOLOCK) ON ap.ID = r.ProcessedBy
    WHERE @IsLeaderOfStandByTeam = 1
        AND EXISTS (
            SELECT 1
            FROM @RequestTypeId
            WHERE [@RequestTypeId].RequestType = r.RequestType
            )
        AND (
            @empid IS NULL
            OR r.empid = @empid
            )
        AND r.STATUS <> 3
        AND (
            e.isDeleted IS NULL
            OR e.isDeleted = 0
            )
        AND (
            @excludedEmpID IS NULL
            OR r.empid <> @excludedEmpID
            )
        AND (
            @status IS NULL
            OR r.STATUS = @status
            )
        AND @role IN (1, 2)
        AND (
            r.RequestType IN (1, 2)
            AND DATEPART(DW, c.WorkDate) IN (1, 7)
			AND g.Code IN (SELECT val FROM fnc_strSplit(@StandByTeam, ','))
            )
		AND (@groupid IS NULL
				OR r.EmpId IN (SELECT Id FROM dbo.Employee WITH (NOLOCK) WHERE GroupId = @groupid)
			)
        AND (
            (
                @isFilterByEffectedDate = 1
                AND (
                    (
                        r.ToDate IS NULL
                        AND DATEDIFF(D, @fromdate, r.FromDate) >= 0
                        AND DATEDIFF(D, r.FromDate, @todate) >= 0
                        )
                    OR (
                        NOT (
                            DATEDIFF(D, @todate, r.FromDate) > 0
                            OR DATEDIFF(D, r.ToDate, @fromdate) > 0
                            )
                        )
                    )
                )
            OR (
                @isFilterByEffectedDate = 0
                AND (
                    DATEDIFF(D, @fromdate, r.CreatedDate) >= 0
                    AND DATEDIFF(D, r.CreatedDate, @todate) >= 0
                    )
                )
            )
        AND (
            @leaveTypeId IS NULL
            OR r.LeaveTypeID = @leaveTypeId
            )
    ORDER BY r.CreatedDate DESC
        ,r.FromDate DESC

    IF @PageIndex IS NOT NULL
        AND @ItemsPerPage IS NOT NULL
    BEGIN
        SELECT t.*
        FROM #WorkingTimeRequest AS t
        ORDER BY t.CreatedDate DESC
            ,t.FromDate DESC OFFSET(@PageIndex * @ItemsPerPage) ROWS

        FETCH NEXT @ItemsPerPage ROWS ONLY;
    END
    ELSE
    BEGIN
        SELECT t.*
        FROM #WorkingTimeRequest AS t
        ORDER BY t.CreatedDate DESC
            ,t.FromDate DESC
    END

    SELECT @TotalItem = COUNT(*)
    FROM #WorkingTimeRequest

    IF OBJECT_ID('tempdb..#WorkingTimeRequest') IS NOT NULL
    BEGIN
        DROP TABLE #WorkingTimeRequest
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_GetRequestHistory]
    @requestid bigint
AS
/*
Created: 20150730@Quinn.Vo
Task: Get requests by all types.
DB: NexHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	DECLARE @IsAllowUpdateStatus TINYINT
	SET @IsAllowUpdateStatus = 0
    SELECT
        r.Id,
        r.empID,
        RequestType,
        (CASE 
            WHEN r.RequestType = 5 THEN s1.Code
            ELSE s.Code END
            ) AS FromShift,
        s2.Code as ToShift,
        r.FromDate AS RequestedDate,
        r.FromDate,
        r.ToDate,
        r.TotalDays,
        lv.Id AS LeaveTypeId,
        lv.NAME AS LeaveTypeName,
        case when RequestType = 3 then LateInMins else EarlyOutMins end as TotalMins,    
        Remark,
        r.CreatedDate,
        Status,
        e.NickName AS Requester,
        e.FullName AS FullNameRequester,
        '' AS EmailRequester,
        g.Name AS Team,
        g.Code AS TeamCode,
        ap.NickName AS ProcessedBy,
        ap.FullName AS FullNameProcessedBy,
        '' AS EmailProcessedBy,
        ProcessedDate,
        0.0 AS TotalDaysRequestInMonth,
        @IsAllowUpdateStatus as IsAllowUpdateStatus,
		r.IsCrossDate
    FROM dbo.WorkingTimeRequest_history r WITH (NOLOCK)
        INNER JOIN Employee e WITH (NOLOCK) ON e.ID = r.empID
        INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid=r.empid and c.WorkDate = convert(VARCHAR(10),r.FromDate,111)
        LEFT JOIN Shift s WITH (NOLOCK) ON s.id = c.ShiftID
        LEFT JOIN Employee ap WITH (NOLOCK) 
        ON (
        (ap.ID = r.ProcessedBy AND r.ProcessedBy IS NOT NULL)
        OR (ap.ID = r.CreatedBy AND r.ProcessedBy IS NULL)
        )
        LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.Id= r.FromShift
        LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.Id= r.ToShift
        LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.Id = r.LeaveTypeID
        LEFT JOIN EmployeeGroup g WITH (NOLOCK) ON e.GroupID = g.ID
    WHERE r.Id = @requestid
    ORDER BY keyid
END
GO

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

	DECLARE @HRGroupId INT
	SELECT @HRGroupId = Id FROM dbo.EmployeeGroup WITH (NOLOCK) WHERE Code = 'HR-ADM'

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
		
		IF(@EmpId = 88) -- user Thien is auto approve check in/out request.
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
		--			,@ErrMsg = 'You have exceeded the number of allowed Annual Leave Days that can be taken. Please, submit the “Unpaid Leave” instead.'

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

	IF (@LeaveTypeID = @BusinessLeave AND @EmpId = 88)
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

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_RejectRequest] @RequestID BIGINT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2012/04/23@Dan
Task: reject a request form (main SP)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	DECLARE @EmpId INT
		,@RequestType SMALLINT
		,@Status TINYINT
		,@LeaveTypeId INT
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@TotalDays NUMERIC(5, 1)

	SELECT @EmpId = EmpId
		,@RequestType = RequestType
		,@Status = [status]
		,@FromDate = FromDate
		,@ToDate = ToDate
		,@LeaveTypeID = LeaveTypeID
		,@TotalDays = TotalDays
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE Id = @RequestID

	-- if status is not pending, show error
	IF (@Status <> 1)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'This request is not pending so it cannot be rejected'

		RETURN
	END

	-- if status is not pending, show error
	IF (
			@ProcessedBy IS NULL
			OR @ProcessedDate IS NULL
			)
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = '@ProcessedBy or @ProcessedDate parameter is missing'

		RETURN
	END

	-- update status of request
	UPDATE dbo.WorkingTimeRequest
	SET [Status] = 3
		,-- rejected
		ProcessedBy = @ProcessedBy
		,ProcessedDate = @ProcessedDate
		,ModifiedDate = @ProcessedDate
		,ModifiedBy = @ProcessedBy
	WHERE Id = @RequestID

	SET @ErrCode = 0

	-- write to history
	EXEC [WorkingTimeRequest_WriteHistory] @RequestID

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
		,@LeaveTypeID = LeaveTypeID
		,@TotalDays = TotalDays
		,@RequestType = RequestType
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE Id = @RequestID

	IF (@RequestType = 6)
	BEGIN
		EXEC [dbo].[WorkingTimeRequest_DeleteLeaveRequest_UpdateCalendar] @RequestID
			,@EmpId
			,@LeaveTypeId
			,@FromDate
			,@ToDate
			,@TotalDays
			,@ProcessedBy

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
	END

	-- write log
	DECLARE @LogSPData VARCHAR(4000)
		,@now DATETIME

	SET @now = dbo.fnc_GetClientDateTime()

	--Log general info
	SELECT @LogSPData = '<WorkingTimeRequest_RejectRequest RequestID="' + coalesce(cast(@RequestID AS VARCHAR(50)), space(1)) + '" ' + 'ProcessedBy="' + coalesce(cast(@ProcessedBy AS VARCHAR(100)), space(1)) + '" ' + 'ProcessedDate="' + coalesce(convert(VARCHAR(10), @ProcessedDate, 111), space(1)) + '" ' + 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' + 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 2
		,@LogDate = @now
		,@Description = 'Reject Request'
		,@TrackingValue = @LogSPData
		,@UserID = @ProcessedBy
END
GO

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

	DECLARE @HRGroupId INT
	SELECT @HRGroupId = Id FROM dbo.EmployeeGroup WITH (NOLOCK) WHERE Code = 'HR-ADM'

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
		
		IF(@EmpId = 88) -- user Thien is auto approve check in/out request.
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
				,@ErrMsg = 'You have exceeded the number of allowed Annual Leave Days that can be taken. Please, submit the “Unpaid Leave” instead.'

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

	IF (@LeaveTypeID = @BusinessLeave AND @EmpId = 88)
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

CREATE OR ALTER PROCEDURE [dbo].[WorkingTimeRequest_WriteHistory]    
    @RequestID bigint
AS
/*
Created: 2012/07/09@Dan
Task: Copy request to history 
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

INSERT dbo.WorkingTimeRequest_history
        (
            Id,
            EmpId,
            RequestType,
            Status,
            IsPlanned,
            Remark,
            ProcessedDate,
            ProcessedBy,
            CreatedDate,
            CreatedBy,
            ModifiedDate,
            ModifiedBy,
            FromDate,
            FromShift,
            ToDate,
            ToShift,
            LeaveTypeID,
            TotalDays,
            LateInMins,
            EarlyOutMins,
            LogDate,
			IsCrossDate
        )
    SELECT
            Id,
            EmpId,
            RequestType,
            Status,
            IsPlanned,
            Remark,
            ProcessedDate,
            ProcessedBy,
            CreatedDate,
            CreatedBy,
            ModifiedDate,
            ModifiedBy,    
            FromDate,
            FromShift,
            ToDate,
            ToShift,
            LeaveTypeID,
            TotalDays,
            LateInMins,
            EarlyOutMins,    
            dbo.fnc_GetClientDateTime(),
			IsCrossDate
    FROM dbo.WorkingTimeRequest
    WHERE Id = @RequestID

GO
