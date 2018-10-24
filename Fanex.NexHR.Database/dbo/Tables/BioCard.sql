CREATE TABLE [dbo].[BioCard] (
    [EmpId]           INT          NOT NULL,
    [nCardIdn]        INT          NOT NULL,
    [nUserIdn]        INT          NOT NULL,
    [nType]           SMALLINT     NOT NULL,
    [sCardNo]         VARCHAR (64) NOT NULL,
    [sCustomNo]       VARCHAR (64) NOT NULL,
    [nSecurityLevel]  INT          NOT NULL,
    [nDateTime]       INT          NOT NULL,
    [nExpiryDateTime] INT          NOT NULL,
    [nBypass]         SMALLINT     NOT NULL,
    [nStatus]         INT          NOT NULL,
    [nLatest]         SMALLINT     NOT NULL,
    [nEncryption]     INT          NOT NULL,
    [nV2CardType]     INT          DEFAULT ((0)) NOT NULL,
    [Status]          SMALLINT     NOT NULL,
    [LastUpdatedAt]   DATETIME     NOT NULL,
    [LastUpdatedBy]   INT          NOT NULL,
    CONSTRAINT [PK_BioCard_1] PRIMARY KEY CLUSTERED ([nCardIdn] ASC)
);

