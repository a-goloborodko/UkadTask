CREATE TABLE [dbo].[Hosts] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Host] NVARCHAR (250) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

