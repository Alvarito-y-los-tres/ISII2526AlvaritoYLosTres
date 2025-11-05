SET IDENTITY_INSERT [dbo].[Reparaciones] ON
INSERT INTO [dbo].[Reparaciones] ([Id], [FechaEntrega], [FechaRecogida], [PrecioTotal], [MetodoPago], [ApplicationUserId]) VALUES (2, N'2010-10-10 10:10:10', N'2011-11-11 11:11:11', CAST(10.00 AS Decimal(8, 2)), 1, N'1')
INSERT INTO [dbo].[Reparaciones] ([Id], [FechaEntrega], [FechaRecogida], [PrecioTotal], [MetodoPago], [ApplicationUserId]) VALUES (3, N'2012-12-12 12:12:12', N'2012-12-13 13:13:13', CAST(13.00 AS Decimal(8, 2)), 2, N'2')
INSERT INTO [dbo].[Reparaciones] ([Id], [FechaEntrega], [FechaRecogida], [PrecioTotal], [MetodoPago], [ApplicationUserId]) VALUES (5, N'2013-07-23 12:34:21', N'2014-07-27 12:30:00', CAST(200.00 AS Decimal(8, 2)), 1, N'3')
SET IDENTITY_INSERT [dbo].[Reparaciones] OFF
