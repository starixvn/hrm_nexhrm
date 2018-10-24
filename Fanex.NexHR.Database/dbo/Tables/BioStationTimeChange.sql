CREATE TABLE [dbo].[BioStationTimeChange] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [nEventLogIdn]    INT           NOT NULL,
    [DeviceId]        INT           NOT NULL,
    [TimeChange]      SMALLDATETIME NOT NULL,
    [PreviousLogTime] SMALLDATETIME NOT NULL,
    [NextLogTime]     SMALLDATETIME NOT NULL,
    [InsertedDate]    DATETIME      NULL,
    CONSTRAINT [PK_BioStationTimeChange] PRIMARY KEY CLUSTERED ([Id] ASC)
);

