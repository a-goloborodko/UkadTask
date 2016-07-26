CREATE TABLE [dbo].[History] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [PageId]       INT      NOT NULL,
    [ResponseTime] INT      NOT NULL,
    [Date]         DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [fk_history_pages] FOREIGN KEY ([PageId]) REFERENCES [dbo].[Pages] ([Id]) ON DELETE CASCADE
);

