CREATE TABLE [dbo].[Pages] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [HostId]          INT            NOT NULL,
    [Url]             NVARCHAR (400) NOT NULL,
    [MinResponseTime] INT            NOT NULL,
    [MaxResponseTime] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [fk_pages_hosts] FOREIGN KEY ([HostId]) REFERENCES [dbo].[Hosts] ([Id]) ON DELETE CASCADE
);

