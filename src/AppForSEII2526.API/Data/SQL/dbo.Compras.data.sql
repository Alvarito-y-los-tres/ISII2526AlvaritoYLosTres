SET IDENTITY_INSERT [dbo].[Compras] ON
INSERT INTO [dbo].[Compras] ([Id], [DireccionEnvio], [FechaCompra], [PrecioTotal], [MetodoPago], [ApplicationUserId]) VALUES (3, N'Calle Tejares', N'2025-06-12 14:04:12', CAST(35.00 AS Decimal(5, 2)), 1, N'1')
INSERT INTO [dbo].[Compras] ([Id], [DireccionEnvio], [FechaCompra], [PrecioTotal], [MetodoPago], [ApplicationUserId]) VALUES (4, N'Calle Moreras', N'2025-09-26 17:56:18', CAST(3.00 AS Decimal(5, 2)), 2, N'2')
INSERT INTO [dbo].[Compras] ([Id], [DireccionEnvio], [FechaCompra], [PrecioTotal], [MetodoPago], [ApplicationUserId]) VALUES (5, N'Calle Nuñez', N'2024-12-19 18:54:12', CAST(5.00 AS Decimal(5, 2)), 1, N'3')
SET IDENTITY_INSERT [dbo].[Compras] OFF
