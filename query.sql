CREATE TABLE [dbo].[Hosts] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Host] NVARCHAR (250) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


CREATE TABLE [dbo].[Pages] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [HostId]          INT            NOT NULL,
    [Url]             NVARCHAR (400) NOT NULL,
    [MinResponseTime] INT            NOT NULL,
    [MaxResponseTime] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [fk_pages_hosts] FOREIGN KEY ([HostId]) REFERENCES [dbo].[Hosts] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[History] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [PageId]       INT      NOT NULL,
    [ResponseTime] INT      NOT NULL,
    [Date]         DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [fk_history_pages] FOREIGN KEY ([PageId]) REFERENCES [dbo].[Pages] ([Id]) ON DELETE CASCADE
);

