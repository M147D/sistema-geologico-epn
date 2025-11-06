-- Script para crear ubicaciones de ejemplo con coordenadas reales de Ecuador
-- Estas ubicaciones pueden ser usadas para vincular elementos geológicos

-- Asegurar que existe al menos un país y provincia
INSERT INTO Paises (NombrePais, CodigoPais, EstadoActivo, FechaCreacion)
SELECT 'Ecuador', 'EC', 1, GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM Paises WHERE NombrePais = 'Ecuador');

DECLARE @PaisId INT = (SELECT Id FROM Paises WHERE NombrePais = 'Ecuador');

INSERT INTO Provincias (NombreProvincia, PaisId, EstadoActivo, FechaCreacion)
SELECT 'Pichincha', @PaisId, 1, GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM Provincias WHERE NombreProvincia = 'Pichincha' AND PaisId = @PaisId);

INSERT INTO Provincias (NombreProvincia, PaisId, EstadoActivo, FechaCreacion)
SELECT 'Azuay', @PaisId, 1, GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM Provincias WHERE NombreProvincia = 'Azuay' AND PaisId = @PaisId);

INSERT INTO Provincias (NombreProvincia, PaisId, EstadoActivo, FechaCreacion)
SELECT 'Tungurahua', @PaisId, 1, GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM Provincias WHERE NombreProvincia = 'Tungurahua' AND PaisId = @PaisId);

-- Obtener IDs de provincias
DECLARE @PichinchaId INT = (SELECT Id FROM Provincias WHERE NombreProvincia = 'Pichincha' AND PaisId = @PaisId);
DECLARE @AzuayId INT = (SELECT Id FROM Provincias WHERE NombreProvincia = 'Azuay' AND PaisId = @PaisId);
DECLARE @TungurahuaId INT = (SELECT Id FROM Provincias WHERE NombreProvincia = 'Tungurahua' AND PaisId = @PaisId);

-- Insertar ubicaciones con coordenadas reales de Ecuador
INSERT INTO Ubicaciones (PaisId, ProvinciaId, Latitud, Longitud, Localidad, Leyenda, EstadoActivo, FechaCreacion)
VALUES 
    -- Quito, Pichincha (volcanes y formaciones geológicas)
    (@PaisId, @PichinchaId, '-0.1807', '-78.4678', 'Quito - Centro Histórico', 'Formaciones volcánicas del Pleistoceno', 1, GETDATE()),
    (@PaisId, @PichinchaId, '-0.2298', '-78.5249', 'Volcán Pichincha', 'Estratovolcán activo, formaciones andesíticas', 1, GETDATE()),
    (@PaisId, @PichinchaId, '0.0219', '-78.4357', 'Mitad del Mundo', 'Línea ecuatorial, formaciones sedimentarias', 1, GETDATE()),
    
    -- Cuenca, Azuay (yacimientos paleontológicos y minerales)
    (@PaisId, @AzuayId, '-2.9001', '-79.0059', 'Cuenca - Centro histórico', 'Formaciones sedimentarias del Paleógeno', 1, GETDATE()),
    (@PaisId, @AzuayId, '-2.8954', '-78.9897', 'Parque Nacional Cajas', 'Formaciones metamórficas precámbricas', 1, GETDATE()),
    (@PaisId, @AzuayId, '-2.7834', '-78.8428', 'Sigsig', 'Yacimientos auríferos y minerales', 1, GETDATE()),
    
    -- Baños, Tungurahua (actividad volcánica)
    (@PaisId, @TungurahuaId, '-1.3928', '-78.4269', 'Baños de Agua Santa', 'Fuentes termales volcánicas', 1, GETDATE()),
    (@PaisId, @TungurahuaId, '-1.4670', '-78.4419', 'Volcán Tungurahua', 'Estratovolcán activo, lavas andesíticas', 1, GETDATE()),
    (@PaisId, @TungurahuaId, '-1.2463', '-78.6179', 'Ambato', 'Valle interandino, depósitos volcánicos', 1, GETDATE());

-- Mostrar las ubicaciones creadas
SELECT 
    u.Id,
    u.Latitud,
    u.Longitud,
    u.Localidad,
    u.Leyenda,
    pr.NombreProvincia,
    p.NombrePais
FROM Ubicaciones u
JOIN Provincias pr ON u.ProvinciaId = pr.Id
JOIN Paises p ON u.PaisId = p.Id
WHERE u.EstadoActivo = 1
ORDER BY u.Id;