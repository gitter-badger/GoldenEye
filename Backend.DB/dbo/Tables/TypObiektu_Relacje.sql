﻿CREATE TABLE [dbo].[TypObiektu_Relacje] (
    [TableID]               INT            NULL,
    [TypObiektuID_L]        INT            NOT NULL,
    [TypObiektuID_R]        INT            NOT NULL,
    [ObiektID_L]            INT            NULL,
    [ObiektID_R]            INT            NULL,
    [RelacjaID]             INT            NOT NULL,
    [PowiazanieID]          INT            NOT NULL,
    [StatusID]              INT            NOT NULL,
    [RzeczywistaDataZmiany] DATETIME       NULL,
    [ZmianaOd]              DATETIME       NULL,
    [ZmianaDo]              DATETIME       NULL,
    [IsValid]               BIT            CONSTRAINT [DF_ObiektyRelacje_IsValid] DEFAULT ((1)) NULL,
    [ValidFrom]             DATETIME       CONSTRAINT [DF_ObiektyRelacje_ValidFrom] DEFAULT (getdate()) NOT NULL,
    [ValidTo]               DATETIME       NULL,
    [PolaczenieID]          INT            NULL,
    [Nazwa]                 NVARCHAR (512) NULL,
    [IsAlternativeHistory]  BIT            DEFAULT ((0)) NULL,
    [IsMainHistFlow]        BIT            DEFAULT ((0)) NULL,
    [IsDeleted]             BIT            DEFAULT ((0)) NOT NULL,
    [DeletedFrom]           DATETIME       NULL,
    [DeletedBy]             INT            NULL,
    [CreatedOn]             DATETIME       DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             INT            NULL,
    [LastModifiedOn]        DATETIME       NULL,
    [LastModifiedBy]        INT            NULL,
    [RealCreatedOn]         DATETIME       DEFAULT (getdate()) NOT NULL,
    [RealLastModifiedOn]    DATETIME       NULL,
    [RealDeletedFrom]       DATETIME       NULL,
    [RealArchivedFrom]      DATETIME       NULL,
    CONSTRAINT [CHK_TypObiektu_Relacje_ObiektID_L] CHECK ([ObiektID_L]>(0)),
    CONSTRAINT [CHK_TypObiektu_Relacje_ObiektID_R] CHECK ([ObiektID_R]>(0)),
    CONSTRAINT [FK_TypObiektu_Relacje_Relacja] FOREIGN KEY ([RelacjaID]) REFERENCES [dbo].[Relacje] ([Id]),
    CONSTRAINT [FK_TypObiektuL] FOREIGN KEY ([TypObiektuID_L]) REFERENCES [dbo].[TypObiektu] ([TypObiekt_ID]),
    CONSTRAINT [FK_TypObiektuR] FOREIGN KEY ([TypObiektuID_R]) REFERENCES [dbo].[TypObiektu] ([TypObiekt_ID])
);
