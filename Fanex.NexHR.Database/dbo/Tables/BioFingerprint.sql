CREATE TABLE [dbo].[BioFingerprint] (
    [EmpId]          INT          NOT NULL,
    [nUserIdn]       INT          NOT NULL,
    [nIndex]         SMALLINT     NOT NULL,
    [bTemplate]      BINARY (384) NOT NULL,
    [nTemplatecs]    INT          NOT NULL,
    [nDuress]        SMALLINT     NOT NULL,
    [nEncryption]    SMALLINT     NOT NULL,
    [nSecurityLevel] INT          NOT NULL,
    [nFingerIndex]   INT          NOT NULL,
    [nTemplateIndex] SMALLINT     NOT NULL,
    [nEnrollQuality] INT          NOT NULL,
    [Status]         SMALLINT     NOT NULL,
    [LastUpdatedAt]  DATETIME     NOT NULL,
    [LastUpdatedBy]  INT          NOT NULL
);

