SET IDENTITY_INSERT [dbo].[Herramientas] ON
INSERT INTO [dbo].[Herramientas] ([Id], [Nombre], [Material], [Precio], [TiempoReparacion], [FabricanteId]) VALUES (1, N'Martillo', N'Madera', CAST(5.00 AS Decimal(18, 2)), 3, 1)
INSERT INTO [dbo].[Herramientas] ([Id], [Nombre], [Material], [Precio], [TiempoReparacion], [FabricanteId]) VALUES (2, N'Taladro', N'Acero', CAST(30.00 AS Decimal(18, 2)), 14, 2)
INSERT INTO [dbo].[Herramientas] ([Id], [Nombre], [Material], [Precio], [TiempoReparacion], [FabricanteId]) VALUES (3, N'Alicates', N'Hierro', CAST(4.00 AS Decimal(18, 2)), 5, 3)
SET IDENTITY_INSERT [dbo].[Herramientas] OFF