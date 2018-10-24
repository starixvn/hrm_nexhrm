CREATE TABLE [dbo].[BioDevice] (
    [DeviceId]     INT          NOT NULL,
    [DisplayName]  VARCHAR (50) NULL,
    [StatusId]     VARCHAR (5)  NULL,
    [ModifiedBy]   INT          NULL,
    [ModifiedDate] DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([DeviceId] ASC)
);

