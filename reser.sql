-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Versión del servidor:         8.0.40 - MySQL Community Server - GPL
-- SO del servidor:              Win64
-- HeidiSQL Versión:             12.10.0.7000
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Volcando estructura de base de datos para reservas
CREATE DATABASE IF NOT EXISTS `reservas` /*!40100 DEFAULT CHARACTER SET utf8mb3 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `reservas`;

-- Volcando estructura para procedimiento reservas.actualizar_datos
DELIMITER //
CREATE PROCEDURE `actualizar_datos`(
	IN `u_id` BIGINT,
	IN `u_nombre` VARCHAR(50),
	IN `u_apellido` VARCHAR(50),
	IN `u_correo` VARCHAR(50),
	IN `u_telefono` BIGINT
)
BEGIN
	UPDATE usuario
	SET usuario.nombre = u_nombre, usuario.apellido = u_apellido, usuario.correo = u_correo, usuario.telefono = u_telefono
	WHERE usuario.numero_doc = u_id;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.actualizar_infoEmpresa
DELIMITER //
CREATE PROCEDURE `actualizar_infoEmpresa`(
	IN `p_nombre` VARCHAR(50),
	IN `p_telefono` BIGINT,
	IN `p_correo` VARCHAR(100),
	IN `p_direccion` VARCHAR(100),
	IN `p_descripcion` VARCHAR(150),
	IN `p_fk_user` INT
)
BEGIN
	INSERT INTO informacion_empresa (nombre, telefono, correo, direccion, descripcion, fk_usuario)
	VALUES
	(p_nombre, p_telefono, p_correo, p_direccion, p_descripcion, p_fk_user);
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.actualizar_multa
DELIMITER //
CREATE PROCEDURE `actualizar_multa`(
	IN `p_id_multa` INT,
	IN `p_fk_tipo` INT,
	IN `p_descripcion` TEXT
)
BEGIN
	UPDATE multa
	SET multa.descripcion = p_descripcion, multa.fk_tp_Multa = p_fk_tipo
	WHERE multa.Pk_multa = p_id_multa;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.actualizar_RE
DELIMITER //
CREATE PROCEDURE `actualizar_RE`(
	IN `p_usuario` BIGINT,
	IN `p_estado` VARCHAR(50),
	IN `p_rol` INT
)
BEGIN
	UPDATE usuario
	SET usuario.estado = p_estado, usuario.Fk_rol = p_rol
	WHERE usuario.numero_doc = p_usuario;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.actualizar_reserva
DELIMITER //
CREATE PROCEDURE `actualizar_reserva`(
	IN `p_reserva` INT,
	IN `p_estado` INT
)
BEGIN
	UPDATE reserva
	SET reserva.fk_estado_reserva = p_estado
	WHERE reserva.PK_reserva = p_reserva;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.asignar_permiso_rol
DELIMITER //
CREATE PROCEDURE `asignar_permiso_rol`(
	IN `p_id_rol` INT,
	IN `p_id_permiso` INT
)
BEGIN
	SELECT @MAX_ID:=MAX(PK_ID_PERMISO_ROL) FROM permiso_rol pr
	WHERE pr.PFK_idrol = p_id_rol AND pr.PFK_idpermiso = p_id_permiso;
	IF (@MAX_ID IS NULL) THEN
		INSERT INTO permiso_rol(PFK_idrol,PFK_idpermiso,ESTADO_PERMISO_ROL)
		VALUES(p_id_rol,p_id_permiso,TRUE);
	ELSE
		SELECT @ESTADO:=ESTADO_PERMISO_ROL FROM permiso_rol pr
		WHERE pr.PFK_idrol = p_id_rol AND pr.PFK_idpermiso = p_id_permiso;
		
		IF (@ESTADO != true) THEN
			UPDATE permiso_rol pr SET ESTADO_PERMISO_ROL = 1
			WHERE pr.PFK_idrol = p_id_rol AND pr.PFK_idpermiso = p_id_permiso;
		ELSE
			UPDATE permiso_rol pr SET ESTADO_PERMISO_ROL = 0
			WHERE pr.PFK_idrol = p_id_rol AND pr.PFK_idpermiso = p_id_permiso;
		END IF;
	END IF;
END//
DELIMITER ;

-- Volcando estructura para tabla reservas.auditoria_empresa
CREATE TABLE IF NOT EXISTS `auditoria_empresa` (
  `id_auditoria` int NOT NULL AUTO_INCREMENT,
  `registro_nuevo` json NOT NULL,
  `fecha_registro` date NOT NULL DEFAULT (curdate()),
  `usuario` int NOT NULL,
  PRIMARY KEY (`id_auditoria`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.auditoria_empresa: ~0 rows (aproximadamente)

-- Volcando estructura para procedimiento reservas.cancelar_reserva
DELIMITER //
CREATE PROCEDURE `cancelar_reserva`(
	IN `p_id_reserva` INT
)
BEGIN
	DELETE FROM reserva
	WHERE reserva.PK_reserva = p_id_reserva AND reserva.fk_estado_reserva = 2;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_agenda_recurso
DELIMITER //
CREATE PROCEDURE `consultar_agenda_recurso`(
	IN `p_id_recurso` INT
)
BEGIN
	IF p_id_recurso != 0 THEN
		SELECT recurso.PK_recurso, horario.PK_horario, dia.nombre AS 'dia', horario.fecha, horario.hora_inicio AS hora, horario.duracion, horario.costo, horario.estado
			FROM recurso
			INNER JOIN horario ON horario.fk_recurso = recurso.PK_recurso
			INNER JOIN dia ON horario.fk_dia = dia.PK_dia
			WHERE recurso.PK_recurso = p_id_recurso AND horario.estado != 'Finalizado'
			ORDER BY horario.fecha ASC, horario.hora_inicio ASC;
	ELSE
		SELECT recurso.PK_recurso, horario.PK_horario, dia.nombre AS 'dia', horario.fecha, horario.hora_inicio AS hora, horario.duracion, horario.costo, horario.estado
			FROM recurso 
			INNER JOIN horario ON horario.fk_recurso = recurso.PK_recurso
			INNER JOIN dia ON horario.fk_dia = dia.PK_dia
			WHERE horario.estado != 'Finalizado'
			ORDER BY recurso.PK_recurso ASC, horario.fecha ASC, horario.hora_inicio ASC;
	END IF;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_disponibilidad
DELIMITER //
CREATE PROCEDURE `consultar_disponibilidad`(
	IN `p_id_recurso` INT,
	IN `p_fecha_inicio` DATE,
	IN `p_fecha_fin` DATE,
	IN `p_hora_inicio` TIME,
	IN `p_hora_fin` TIME
)
BEGIN
	SELECT * FROM horario
	WHERE horario.fk_recurso = p_id_recurso AND horario.estado = 'Disponible' AND horario.fecha BETWEEN p_fecha_inicio AND p_fecha_fin 
	AND horario.hora_inicio BETWEEN p_hora_inicio AND p_hora_fin
	ORDER BY horario.fecha ASC, horario.hora_inicio ASC;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_multa
DELIMITER //
CREATE PROCEDURE `consultar_multa`(
	IN `p_id_multa` INT
)
BEGIN
	IF p_id_multa = 0 THEN
		SELECT * FROM multa;
	ELSE 
		SELECT * FROM multa
		WHERE multa.Pk_multa = p_id_multa;
END IF;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_permisos
DELIMITER //
CREATE PROCEDURE `consultar_permisos`(
	IN `p_id_rol` INT
)
BEGIN
	IF p_id_rol = 0 THEN
		SELECT *,'False' AS 'ESTADO_PERMISO_ROL' FROM permiso p;
	ELSE  
      SELECT * FROM permiso 
      LEFT JOIN permiso_rol ON permiso_rol.PFK_idrol = p_id_rol AND permiso.Pk_permiso = permiso_rol.PFK_idpermiso;
	END IF;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_recurso
DELIMITER //
CREATE PROCEDURE `consultar_recurso`(
	IN `p_id_recurso` INT
)
BEGIN
IF p_id_recurso = 0 THEN
	SELECT recurso.PK_recurso, recurso.nombre, recurso.estado, recurso.Direccion, tipo_recurso.nombre AS nombre_recurso, 
	CONCAT(usuario.nombre, ' ',usuario.apellido) AS nombre_usuario
	FROM recurso
	INNER JOIN tipo_recurso ON recurso.FK_tp_recurso = tipo_recurso.PK_tp_recurso
	LEFT JOIN usuario ON recurso.fk_usuario_encargado = usuario.numero_doc
	ORDER BY recurso.PK_recurso;
ELSE
	SELECT recurso.PK_recurso, recurso.nombre, recurso.estado, recurso.Direccion, tipo_recurso.nombre AS nombre_recurso, 
	CONCAT(usuario.nombre, ' ',usuario.apellido) AS nombre_usuario
	FROM recurso
	INNER JOIN tipo_recurso ON recurso.FK_tp_recurso = tipo_recurso.PK_tp_recurso
	INNER JOIN usuario ON recurso.fk_usuario_encargado = usuario.numero_doc
	WHERE recurso.PK_recurso = p_id_recurso
	ORDER BY recurso.PK_recurso;
END IF;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_reserva
DELIMITER //
CREATE PROCEDURE `consultar_reserva`(
	IN `p_id_reserva` INT
)
BEGIN
	SELECT reserva.PK_reserva AS 'ID', usuario.numero_doc, usuario.nombre, usuario.apellido, usuario.telefono, usuario.correo, 
	horario.costo, reserva.fecha_registro, reserva.fk_estado_reserva
	FROM horario
	INNER JOIN reserva
	ON horario.fk_reserva = reserva.PK_reserva
	INNER JOIN usuario
	ON reserva.fk_usuario_cliente = usuario.numero_doc
		WHERE reserva.PK_reserva = p_id_reserva
	ORDER BY reserva.fecha_registro;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_reservas
DELIMITER //
CREATE PROCEDURE `consultar_reservas`()
BEGIN
	SELECT reserva.PK_reserva, recurso.nombre, usuario.numero_doc, horario.fecha, horario.hora_inicio, horario.costo, 
		estado_reserva.nombre, reserva.fecha_registro
	FROM recurso
	INNER JOIN horario
	ON horario.fk_recurso = recurso.PK_recurso
	INNER JOIN reserva
	ON horario.fk_reserva = reserva.PK_reserva
	INNER JOIN estado_reserva
	ON reserva.fk_estado_reserva = estado_reserva.Pk_estado_reserva
	INNER JOIN usuario
	ON reserva.fk_usuario_cliente = usuario.numero_doc
		WHERE reserva.fk_estado_reserva != 4
	ORDER BY reserva.fecha_registro;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_reser_recurso
DELIMITER //
CREATE PROCEDURE `consultar_reser_recurso`(
	IN `p_id_recurso` INT
)
BEGIN
	IF p_id_recurso != 0 THEN
		SELECT recurso.nombre AS 'recurso', usuario.nombre, usuario.apellido, reserva.fecha_registro, horario.fecha, horario.hora_inicio, horario.duracion, horario.costo, estado_reserva.nombre	AS 'estado',
			horario.PK_horario, reserva.PK_reserva AS ID, recurso.PK_recurso, usuario.numero_doc
			FROM recurso
			INNER JOIN horario
			ON horario.fk_recurso = recurso.PK_recurso
			INNER JOIN reserva
			ON horario.fk_reserva = reserva.PK_reserva
			INNER JOIN usuario
			ON reserva.fk_usuario_cliente = usuario.numero_doc
			INNER JOIN estado_reserva
			ON reserva.fk_estado_reserva = estado_reserva.Pk_estado_reserva
			WHERE recurso.PK_recurso = p_id_recurso
		ORDER BY horario.fecha;
	ELSE
		SELECT recurso.nombre AS 'recurso', usuario.nombre, usuario.apellido, reserva.fecha_registro, horario.fecha, horario.hora_inicio, horario.duracion, horario.costo, estado_reserva.nombre AS 'estado',
			horario.PK_horario, reserva.PK_reserva AS ID, recurso.PK_recurso, usuario.numero_doc
			FROM recurso
			INNER JOIN horario
			ON horario.fk_recurso = recurso.PK_recurso
			INNER JOIN reserva
			ON horario.fk_reserva = reserva.PK_reserva
			INNER JOIN usuario
			ON reserva.fk_usuario_cliente = usuario.numero_doc
			INNER JOIN estado_reserva
			ON reserva.fk_estado_reserva = estado_reserva.Pk_estado_reserva
		ORDER BY horario.fecha;
	END IF;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_rol
DELIMITER //
CREATE PROCEDURE `consultar_rol`(
	IN `p_id_rol` INT
)
BEGIN
IF p_id_rol = 0 THEN
	SELECT rol.PK_rol, rol.nombre, rol.estado
	FROM rol
	WHERE rol.estado = 'Activo';
ELSE
	SELECT rol.PK_rol, rol.nombre, rol.estado
	FROM rol
	WHERE rol.PK_rol = p_id_rol AND rol.estado = 'Activo';
END IF;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_tpmulta
DELIMITER //
CREATE PROCEDURE `consultar_tpmulta`(
	IN `p_id` INT
)
BEGIN
IF p_id = 0 THEN
	SELECT * FROM tipo_multa;
ELSE 
	SELECT * FROM tipo_multa
	WHERE tipo_multa.PK_tipo_multa = p_id;
END IF;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consultar_usuario
DELIMITER //
CREATE PROCEDURE `consultar_usuario`(
	IN `p_id_usuario` BIGINT
)
BEGIN
IF p_id_usuario = 0 THEN
	SELECT usuario.numero_doc, usuario.telefono, usuario.nombre, usuario.apellido, usuario.correo, 
	usuario.fecha_registro, usuario.estado, usuario.Fk_tp_documento, tipo_documento.nombre 
	AS nombre_tp_doc, usuario.Fk_rol, rol.nombre AS nombre_rol
	FROM usuario
	INNER JOIN tipo_documento ON usuario.Fk_tp_documento = tipo_documento.PK_tipo_doc
	INNER JOIN rol ON usuario.Fk_rol = rol.PK_rol
	ORDER BY usuario.fecha_registro ASC;
ELSE
	SELECT usuario.numero_doc, usuario.telefono, usuario.nombre, usuario.apellido, usuario.correo, 
	usuario.fecha_registro, usuario.estado, usuario.Fk_tp_documento, tipo_documento.nombre
	AS nombre_tp_doc, usuario.Fk_rol, rol.nombre AS nombre_rol
	FROM usuario
	INNER JOIN tipo_documento ON usuario.Fk_tp_documento = tipo_documento.PK_tipo_doc
	INNER JOIN rol ON usuario.Fk_rol = rol.PK_rol
	WHERE usuario.numero_doc = p_id_usuario;
END IF;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.consulta_agenda
DELIMITER //
CREATE PROCEDURE `consulta_agenda`(
	IN `pk_usuario` INT,
	IN `fecha` DATE
)
BEGIN
	IF fecha IS NULL THEN
		SELECT horario.PK_horario, dia.nombre AS 'dia', horario.fecha, horario.hora_inicio AS hora, horario.duracion, horario.costo, horario.estado
		FROM usuario
		INNER JOIN recurso ON recurso.fk_usuario_encargado = usuario.numero_doc
		INNER JOIN horario ON horario.fk_recurso = recurso.PK_recurso
		INNER JOIN dia ON horario.fk_dia = dia.PK_dia
		WHERE usuario.numero_doc = pk_usuario AND horario.estado != 'Finalizado'
		ORDER BY horario.fecha DESC, horario.hora_inicio ASC;
	ELSE 
		SELECT horario.PK_horario, dia.nombre AS 'dia', horario.fecha, horario.hora_inicio AS hora, horario.duracion, horario.costo, horario.estado
		FROM usuario
		INNER JOIN recurso ON recurso.fk_usuario_encargado = usuario.numero_doc
		INNER JOIN horario ON horario.fk_recurso = recurso.PK_recurso
		INNER JOIN dia ON horario.fk_dia = dia.PK_dia
		WHERE usuario.numero_doc = pk_usuario AND horario.estado != 'Finalizado' AND horario.fecha = fecha
		ORDER BY horario.fecha DESC, horario.hora_inicio ASC;
	END IF;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.crear_horario
DELIMITER //
CREATE PROCEDURE `crear_horario`(
	IN `p_fecha` DATE,
	IN `p_hora_inicio` TIME,
	IN `p_duracion` INT,
	IN `p_costo` FLOAT,
	IN `p_fk_dia` INT,
	IN `p_fk_recurso` INT
)
BEGIN
INSERT INTO horario (fecha, hora_inicio, duracion, costo, fk_dia, fk_recurso)
VALUES 
(p_fecha, p_hora_inicio, p_duracion, p_costo, p_fk_dia + 1, p_fk_recurso);
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.crear_multa
DELIMITER //
CREATE PROCEDURE `crear_multa`(
	IN `p_descripcion` TEXT,
	IN `p_pfk_reserva` INT,
	IN `p_fk_tp_multa` INT
)
BEGIN
INSERT INTO multa (descripcion, pfk_reserva, fk_tp_multa)
VALUES 
(p_descripcion, p_pfk_reserva, p_fk_tp_multa);
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.crear_recurso
DELIMITER //
CREATE PROCEDURE `crear_recurso`(
	IN `p_nombre` VARCHAR(50),
	IN `p_direccion` VARCHAR(100),
	IN `p_fk_tp_recurso` INT,
	IN `p_fk_usuario_encargado` BIGINT
)
BEGIN
IF p_fk_usuario_encargado != 0 THEN
	INSERT INTO recurso (nombre, direccion, fk_tp_recurso, fk_usuario_encargado)
	VALUES
	(p_nombre, p_direccion, p_fk_tp_recurso, p_fk_usuario_encargado);
ELSE
	INSERT INTO recurso (nombre, direccion, fk_tp_recurso)
	VALUES
	(p_nombre, p_direccion, p_fk_tp_recurso);
	
END if;

END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.crear_reserva
DELIMITER //
CREATE PROCEDURE `crear_reserva`(
	IN `p_fk_usuario` BIGINT,
	IN `p_fk_horario` INT
)
BEGIN
DECLARE nueva_pk_reserva INT;
IF EXISTS (
	SELECT 1
	FROM horario
	WHERE horario.PK_horario = p_fk_horario
) THEN 
	INSERT INTO reserva (fk_usuario_cliente)
	VALUES
	(p_fk_usuario);

	SET nueva_pk_reserva = LAST_INSERT_ID();
	UPDATE horario 
	SET horario.fk_reserva = nueva_pk_reserva, horario.estado = 'Reservado'
    	WHERE pk_horario = p_fk_horario;

ELSE 
	SIGNAL SQLSTATE '45000'
		SET MESSAGE_TEXT = 'No se pudo agregar';

END IF;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.crear_rol
DELIMITER //
CREATE PROCEDURE `crear_rol`(
	IN `nombre` VARCHAR(50)
)
BEGIN
INSERT INTO rol (nombre)
VALUES
(nombre);
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.crear_tp_multa
DELIMITER //
CREATE PROCEDURE `crear_tp_multa`(
	IN `p_nombre` VARCHAR(50),
	IN `p_valor` FLOAT,
	IN `p_dias` INT
)
BEGIN
INSERT INTO tipo_multa (nombre, valor, dias)
VALUES 
(p_nombre, p_valor, p_dias);
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.crear_usuario
DELIMITER //
CREATE PROCEDURE `crear_usuario`(
	IN `numero_doc` BIGINT,
	IN `telefono` BIGINT,
	IN `nombre` VARCHAR(50),
	IN `apellido` VARCHAR(50),
	IN `correo` VARCHAR(50),
	IN `contrasenia` VARCHAR(50),
	IN `FK_tp_documento` INT,
	IN `token` VARCHAR(200),
	IN `restablecer` BIT,
	IN `FK_rol` INT
)
BEGIN
IF fk_rol = 0 THEN
	INSERT INTO usuario(numero_doc, telefono, nombre, apellido, correo, contrasenia, FK_tp_documento, token, restablecer)
	VALUES
	(numero_doc, telefono, nombre, apellido, correo, contrasenia, FK_tp_documento, token, restablecer);
ELSE
	INSERT INTO usuario(numero_doc, telefono, nombre, apellido, correo, contrasenia, FK_tp_documento, token, FK_rol, restablecer)
	VALUES
	(numero_doc, telefono, nombre, apellido, correo, contrasenia, FK_tp_documento, token, FK_rol, restablecer);
END IF;
END//
DELIMITER ;

-- Volcando estructura para tabla reservas.dia
CREATE TABLE IF NOT EXISTS `dia` (
  `PK_dia` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(45) NOT NULL,
  PRIMARY KEY (`PK_dia`),
  UNIQUE KEY `PK_dia_UNIQUE` (`PK_dia`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.dia: ~7 rows (aproximadamente)
INSERT INTO `dia` (`PK_dia`, `nombre`) VALUES
	(1, 'Domingo'),
	(2, 'Lunes'),
	(3, 'Martes'),
	(4, 'Miercoles'),
	(5, 'Jueves'),
	(6, 'Viernes'),
	(7, 'Sabado');

-- Volcando estructura para procedimiento reservas.eliminar_horario
DELIMITER //
CREATE PROCEDURE `eliminar_horario`(
	IN `p_horario` INT
)
BEGIN
DELETE FROM horario 
WHERE horario.PK_horario = p_horario AND horario.estado ='Disponible';
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.eliminar_multa
DELIMITER //
CREATE PROCEDURE `eliminar_multa`(
	IN `p_id_multa` INT,
	IN `pagar` TINYTEXT
)
BEGIN
	UPDATE usuario
	INNER JOIN reserva ON reserva.fk_usuario_cliente = usuario.numero_doc
	INNER JOIN multa ON multa.PFK_reserva = reserva.PK_reserva
		SET usuario.estado = 'Activo'
		WHERE multa.Pk_multa = p_id_multa AND multa.estado = 'Activa';
	
	IF pagar = 0 THEN
		DELETE FROM multa
		WHERE multa.Pk_multa = p_id_multa AND multa.estado = 'Activa';
	ELSE
		UPDATE multa
		SET multa.estado = 'Finalizada'
		WHERE multa.Pk_multa = p_id_multa;
	END IF;
	
END//
DELIMITER ;

-- Volcando estructura para tabla reservas.estado_reserva
CREATE TABLE IF NOT EXISTS `estado_reserva` (
  `Pk_estado_reserva` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  PRIMARY KEY (`Pk_estado_reserva`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.estado_reserva: ~4 rows (aproximadamente)
INSERT INTO `estado_reserva` (`Pk_estado_reserva`, `nombre`) VALUES
	(1, 'Finalizada'),
	(2, 'Activa'),
	(3, 'No asistió'),
	(4, 'Si asistió');

-- Volcando estructura para evento reservas.finalizar_horario
DELIMITER //
CREATE EVENT `finalizar_horario` ON SCHEDULE EVERY 1 MINUTE STARTS '2025-05-18 17:45:41' ENDS '2025-05-23 17:45:45' ON COMPLETION PRESERVE ENABLE DO BEGIN
UPDATE horario
		SET horario.estado = 'Finalizado'
		WHERE CONCAT(horario.fecha, ' ', horario.hora_inicio) < NOW();


	UPDATE reserva
	INNER JOIN horario
	ON horario.fk_reserva = reserva.PK_reserva
		SET reserva.fk_estado_reserva = 1
		WHERE reserva.fk_estado_reserva = 2 AND CONCAT(horario.fecha, ' ', horario.hora_inicio) < NOW() AND horario.fk_reserva IS NOT NULL;
END//
DELIMITER ;

-- Volcando estructura para evento reservas.finalizar_multa
DELIMITER //
CREATE EVENT `finalizar_multa` ON SCHEDULE EVERY 1 MINUTE STARTS '2023-12-02 17:26:21' ENDS '2025-05-23 07:37:22' ON COMPLETION PRESERVE ENABLE DO BEGIN
	UPDATE multa
		SET multa.estado = 'Finalizada'
		WHERE multa.fecha_fin < NOW();
END//
DELIMITER ;

-- Volcando estructura para tabla reservas.horario
CREATE TABLE IF NOT EXISTS `horario` (
  `PK_horario` int NOT NULL AUTO_INCREMENT,
  `fecha` date NOT NULL,
  `hora_inicio` time NOT NULL,
  `duracion` int NOT NULL DEFAULT '20',
  `costo` float NOT NULL,
  `estado` enum('Disponible','Reservado','Finalizado') CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Disponible',
  `fk_dia` int NOT NULL,
  `fk_recurso` int NOT NULL,
  `fk_reserva` int DEFAULT NULL,
  PRIMARY KEY (`PK_horario`),
  UNIQUE KEY `PK_horario_UNIQUE` (`PK_horario`),
  KEY `fk_Horario_Dia1_idx` (`fk_dia`),
  KEY `fk_Horario_Recurso1_idx` (`fk_recurso`),
  KEY `fk_Horario_Reserva1_idx` (`fk_reserva`),
  CONSTRAINT `fk_Horario_Dia1` FOREIGN KEY (`fk_dia`) REFERENCES `dia` (`PK_dia`),
  CONSTRAINT `fk_Horario_Recurso1` FOREIGN KEY (`fk_recurso`) REFERENCES `recurso` (`PK_recurso`),
  CONSTRAINT `fk_Horario_Reserva1` FOREIGN KEY (`fk_reserva`) REFERENCES `reserva` (`PK_reserva`)
) ENGINE=InnoDB AUTO_INCREMENT=279 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.horario: ~278 rows (aproximadamente)
INSERT INTO `horario` (`PK_horario`, `fecha`, `hora_inicio`, `duracion`, `costo`, `estado`, `fk_dia`, `fk_recurso`, `fk_reserva`) VALUES
	(1, '2025-05-21', '08:16:00', 40, 20000, 'Finalizado', 4, 1, NULL),
	(2, '2025-05-21', '08:56:00', 40, 20000, 'Finalizado', 4, 1, NULL),
	(3, '2025-05-21', '09:36:00', 40, 20000, 'Finalizado', 4, 1, NULL),
	(4, '2025-05-21', '10:16:00', 40, 20000, 'Finalizado', 4, 1, NULL),
	(5, '2025-05-21', '10:56:00', 40, 20000, 'Finalizado', 4, 1, NULL),
	(6, '2025-05-21', '11:36:00', 40, 20000, 'Finalizado', 4, 1, NULL),
	(7, '2025-05-21', '12:16:00', 40, 20000, 'Finalizado', 4, 1, NULL),
	(8, '2025-05-21', '12:56:00', 40, 20000, 'Finalizado', 4, 1, NULL),
	(9, '2025-05-21', '13:36:00', 40, 20000, 'Finalizado', 4, 1, NULL),
	(10, '2025-05-22', '08:16:00', 40, 20000, 'Disponible', 5, 1, NULL),
	(11, '2025-05-22', '08:56:00', 40, 20000, 'Disponible', 5, 1, NULL),
	(12, '2025-05-22', '09:36:00', 40, 20000, 'Disponible', 5, 1, NULL),
	(13, '2025-05-22', '10:16:00', 40, 20000, 'Disponible', 5, 1, NULL),
	(14, '2025-05-22', '10:56:00', 40, 20000, 'Disponible', 5, 1, NULL),
	(15, '2025-05-22', '11:36:00', 40, 20000, 'Disponible', 5, 1, NULL),
	(16, '2025-05-22', '12:16:00', 40, 20000, 'Disponible', 5, 1, NULL),
	(17, '2025-05-22', '12:56:00', 40, 20000, 'Disponible', 5, 1, NULL),
	(18, '2025-05-22', '13:36:00', 40, 20000, 'Disponible', 5, 1, NULL),
	(19, '2025-05-23', '08:16:00', 40, 20000, 'Disponible', 6, 1, NULL),
	(20, '2025-05-23', '08:56:00', 40, 20000, 'Disponible', 6, 1, NULL),
	(21, '2025-05-23', '09:36:00', 40, 20000, 'Disponible', 6, 1, NULL),
	(22, '2025-05-23', '10:16:00', 40, 20000, 'Disponible', 6, 1, NULL),
	(23, '2025-05-23', '10:56:00', 40, 20000, 'Disponible', 6, 1, NULL),
	(24, '2025-05-23', '11:36:00', 40, 20000, 'Disponible', 6, 1, NULL),
	(25, '2025-05-23', '12:16:00', 40, 20000, 'Disponible', 6, 1, NULL),
	(26, '2025-05-23', '12:56:00', 40, 20000, 'Disponible', 6, 1, NULL),
	(27, '2025-05-23', '13:36:00', 40, 20000, 'Disponible', 6, 1, NULL),
	(28, '2025-05-24', '08:16:00', 40, 20000, 'Disponible', 7, 1, NULL),
	(29, '2025-05-24', '08:56:00', 40, 20000, 'Disponible', 7, 1, NULL),
	(30, '2025-05-24', '09:36:00', 40, 20000, 'Disponible', 7, 1, NULL),
	(31, '2025-05-24', '10:16:00', 40, 20000, 'Disponible', 7, 1, NULL),
	(32, '2025-05-24', '10:56:00', 40, 20000, 'Disponible', 7, 1, NULL),
	(33, '2025-05-24', '11:36:00', 40, 20000, 'Disponible', 7, 1, NULL),
	(34, '2025-05-24', '12:16:00', 40, 20000, 'Disponible', 7, 1, NULL),
	(35, '2025-05-24', '12:56:00', 40, 20000, 'Disponible', 7, 1, NULL),
	(36, '2025-05-24', '13:36:00', 40, 20000, 'Disponible', 7, 1, NULL),
	(37, '2025-05-25', '08:16:00', 40, 20000, 'Disponible', 1, 1, NULL),
	(38, '2025-05-25', '08:56:00', 40, 20000, 'Disponible', 1, 1, NULL),
	(39, '2025-05-25', '09:36:00', 40, 20000, 'Disponible', 1, 1, NULL),
	(40, '2025-05-25', '10:16:00', 40, 20000, 'Disponible', 1, 1, NULL),
	(41, '2025-05-25', '10:56:00', 40, 20000, 'Disponible', 1, 1, NULL),
	(42, '2025-05-25', '11:36:00', 40, 20000, 'Disponible', 1, 1, NULL),
	(43, '2025-05-25', '12:16:00', 40, 20000, 'Disponible', 1, 1, NULL),
	(44, '2025-05-25', '12:56:00', 40, 20000, 'Disponible', 1, 1, NULL),
	(45, '2025-05-25', '13:36:00', 40, 20000, 'Disponible', 1, 1, NULL),
	(46, '2025-05-26', '08:16:00', 40, 20000, 'Disponible', 2, 1, NULL),
	(47, '2025-05-26', '08:56:00', 40, 20000, 'Disponible', 2, 1, NULL),
	(48, '2025-05-26', '09:36:00', 40, 20000, 'Disponible', 2, 1, NULL),
	(49, '2025-05-26', '10:16:00', 40, 20000, 'Disponible', 2, 1, NULL),
	(50, '2025-05-26', '10:56:00', 40, 20000, 'Disponible', 2, 1, NULL),
	(51, '2025-05-26', '11:36:00', 40, 20000, 'Disponible', 2, 1, NULL),
	(52, '2025-05-26', '12:16:00', 40, 20000, 'Disponible', 2, 1, NULL),
	(53, '2025-05-26', '12:56:00', 40, 20000, 'Disponible', 2, 1, NULL),
	(54, '2025-05-26', '13:36:00', 40, 20000, 'Disponible', 2, 1, NULL),
	(55, '2025-05-27', '08:16:00', 40, 20000, 'Disponible', 3, 1, NULL),
	(56, '2025-05-27', '08:56:00', 40, 20000, 'Disponible', 3, 1, NULL),
	(57, '2025-05-27', '09:36:00', 40, 20000, 'Disponible', 3, 1, NULL),
	(58, '2025-05-27', '10:16:00', 40, 20000, 'Disponible', 3, 1, NULL),
	(59, '2025-05-27', '10:56:00', 40, 20000, 'Disponible', 3, 1, NULL),
	(60, '2025-05-27', '11:36:00', 40, 20000, 'Disponible', 3, 1, NULL),
	(61, '2025-05-27', '12:16:00', 40, 20000, 'Disponible', 3, 1, NULL),
	(62, '2025-05-27', '12:56:00', 40, 20000, 'Disponible', 3, 1, NULL),
	(63, '2025-05-27', '13:36:00', 40, 20000, 'Disponible', 3, 1, NULL),
	(64, '2025-05-21', '08:00:00', 60, 15000, 'Finalizado', 4, 2, NULL),
	(65, '2025-05-21', '09:00:00', 60, 15000, 'Finalizado', 4, 2, NULL),
	(66, '2025-05-21', '10:00:00', 60, 15000, 'Finalizado', 4, 2, NULL),
	(67, '2025-05-21', '11:00:00', 60, 15000, 'Finalizado', 4, 2, NULL),
	(68, '2025-05-21', '12:00:00', 60, 15000, 'Finalizado', 4, 2, NULL),
	(69, '2025-05-21', '13:00:00', 60, 15000, 'Finalizado', 4, 2, NULL),
	(70, '2025-05-21', '14:00:00', 60, 15000, 'Finalizado', 4, 2, NULL),
	(71, '2025-05-21', '15:00:00', 60, 15000, 'Finalizado', 4, 2, NULL),
	(72, '2025-05-21', '16:00:00', 60, 15000, 'Finalizado', 4, 2, NULL),
	(73, '2025-05-21', '17:00:00', 60, 15000, 'Finalizado', 4, 2, NULL),
	(74, '2025-05-21', '18:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(75, '2025-05-22', '08:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(76, '2025-05-22', '09:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(77, '2025-05-22', '10:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(78, '2025-05-22', '11:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(79, '2025-05-22', '12:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(80, '2025-05-22', '13:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(81, '2025-05-22', '14:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(82, '2025-05-22', '15:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(83, '2025-05-22', '16:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(84, '2025-05-22', '17:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(85, '2025-05-22', '18:00:00', 60, 15000, 'Disponible', 5, 2, NULL),
	(86, '2025-05-23', '08:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(87, '2025-05-23', '09:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(88, '2025-05-23', '10:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(89, '2025-05-23', '11:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(90, '2025-05-23', '12:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(91, '2025-05-23', '13:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(92, '2025-05-23', '14:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(93, '2025-05-23', '15:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(94, '2025-05-23', '16:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(95, '2025-05-23', '17:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(96, '2025-05-23', '18:00:00', 60, 15000, 'Disponible', 6, 2, NULL),
	(97, '2025-05-24', '08:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(98, '2025-05-24', '09:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(99, '2025-05-24', '10:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(100, '2025-05-24', '11:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(101, '2025-05-24', '12:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(102, '2025-05-24', '13:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(103, '2025-05-24', '14:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(104, '2025-05-24', '15:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(105, '2025-05-24', '16:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(106, '2025-05-24', '17:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(107, '2025-05-24', '18:00:00', 60, 15000, 'Disponible', 7, 2, NULL),
	(108, '2025-05-25', '08:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(109, '2025-05-25', '09:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(110, '2025-05-25', '10:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(111, '2025-05-25', '11:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(112, '2025-05-25', '12:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(113, '2025-05-25', '13:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(114, '2025-05-25', '14:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(115, '2025-05-25', '15:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(116, '2025-05-25', '16:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(117, '2025-05-25', '17:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(118, '2025-05-25', '18:00:00', 60, 15000, 'Disponible', 1, 2, NULL),
	(119, '2025-05-26', '08:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(120, '2025-05-26', '09:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(121, '2025-05-26', '10:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(122, '2025-05-26', '11:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(123, '2025-05-26', '12:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(124, '2025-05-26', '13:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(125, '2025-05-26', '14:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(126, '2025-05-26', '15:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(127, '2025-05-26', '16:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(128, '2025-05-26', '17:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(129, '2025-05-26', '18:00:00', 60, 15000, 'Disponible', 2, 2, NULL),
	(130, '2025-05-27', '08:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(131, '2025-05-27', '09:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(132, '2025-05-27', '10:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(133, '2025-05-27', '11:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(134, '2025-05-27', '12:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(135, '2025-05-27', '13:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(136, '2025-05-27', '14:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(137, '2025-05-27', '15:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(138, '2025-05-27', '16:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(139, '2025-05-27', '17:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(140, '2025-05-27', '18:00:00', 60, 15000, 'Disponible', 3, 2, NULL),
	(141, '2025-05-28', '08:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(142, '2025-05-28', '09:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(143, '2025-05-28', '10:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(144, '2025-05-28', '11:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(145, '2025-05-28', '12:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(146, '2025-05-28', '13:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(147, '2025-05-28', '14:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(148, '2025-05-28', '15:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(149, '2025-05-28', '16:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(150, '2025-05-28', '17:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(151, '2025-05-28', '18:00:00', 60, 15000, 'Disponible', 4, 2, NULL),
	(152, '2025-05-21', '09:17:00', 180, 0, 'Finalizado', 4, 3, NULL),
	(153, '2025-05-21', '12:17:00', 180, 0, 'Finalizado', 4, 3, NULL),
	(154, '2025-05-21', '15:17:00', 180, 0, 'Finalizado', 4, 3, NULL),
	(155, '2025-05-21', '18:17:00', 180, 0, 'Disponible', 4, 3, NULL),
	(156, '2025-05-22', '09:17:00', 180, 0, 'Disponible', 5, 3, NULL),
	(157, '2025-05-22', '12:17:00', 180, 0, 'Disponible', 5, 3, NULL),
	(158, '2025-05-22', '15:17:00', 180, 0, 'Disponible', 5, 3, NULL),
	(159, '2025-05-22', '18:17:00', 180, 0, 'Disponible', 5, 3, NULL),
	(160, '2025-05-23', '09:17:00', 180, 0, 'Disponible', 6, 3, NULL),
	(161, '2025-05-23', '12:17:00', 180, 0, 'Disponible', 6, 3, NULL),
	(162, '2025-05-23', '15:17:00', 180, 0, 'Disponible', 6, 3, NULL),
	(163, '2025-05-23', '18:17:00', 180, 0, 'Disponible', 6, 3, NULL),
	(164, '2025-05-24', '09:17:00', 180, 0, 'Disponible', 7, 3, NULL),
	(165, '2025-05-24', '12:17:00', 180, 0, 'Disponible', 7, 3, NULL),
	(166, '2025-05-24', '15:17:00', 180, 0, 'Disponible', 7, 3, NULL),
	(167, '2025-05-24', '18:17:00', 180, 0, 'Disponible', 7, 3, NULL),
	(168, '2025-05-25', '09:17:00', 180, 0, 'Disponible', 1, 3, NULL),
	(169, '2025-05-25', '12:17:00', 180, 0, 'Disponible', 1, 3, NULL),
	(170, '2025-05-25', '15:17:00', 180, 0, 'Disponible', 1, 3, NULL),
	(171, '2025-05-25', '18:17:00', 180, 0, 'Disponible', 1, 3, NULL),
	(172, '2025-05-26', '09:17:00', 180, 0, 'Disponible', 2, 3, NULL),
	(173, '2025-05-26', '12:17:00', 180, 0, 'Disponible', 2, 3, NULL),
	(174, '2025-05-26', '15:17:00', 180, 0, 'Disponible', 2, 3, NULL),
	(175, '2025-05-26', '18:17:00', 180, 0, 'Disponible', 2, 3, NULL),
	(176, '2025-05-27', '09:17:00', 180, 0, 'Disponible', 3, 3, NULL),
	(177, '2025-05-27', '12:17:00', 180, 0, 'Disponible', 3, 3, NULL),
	(178, '2025-05-27', '15:17:00', 180, 0, 'Disponible', 3, 3, NULL),
	(179, '2025-05-27', '18:17:00', 180, 0, 'Disponible', 3, 3, NULL),
	(180, '2025-05-28', '09:17:00', 180, 0, 'Disponible', 4, 3, NULL),
	(181, '2025-05-28', '12:17:00', 180, 0, 'Disponible', 4, 3, NULL),
	(182, '2025-05-28', '15:17:00', 180, 0, 'Disponible', 4, 3, NULL),
	(183, '2025-05-28', '18:17:00', 180, 0, 'Disponible', 4, 3, NULL),
	(184, '2025-05-21', '10:59:00', 30, 50000, 'Finalizado', 4, 3, NULL),
	(185, '2025-05-21', '11:29:00', 30, 50000, 'Finalizado', 4, 3, NULL),
	(186, '2025-05-21', '11:59:00', 30, 50000, 'Finalizado', 4, 3, NULL),
	(187, '2025-05-21', '12:29:00', 30, 50000, 'Finalizado', 4, 3, NULL),
	(188, '2025-05-21', '12:59:00', 30, 50000, 'Finalizado', 4, 3, NULL),
	(189, '2025-05-21', '13:29:00', 30, 50000, 'Finalizado', 4, 3, NULL),
	(190, '2025-05-21', '13:59:00', 30, 50000, 'Finalizado', 4, 3, NULL),
	(191, '2025-05-21', '14:29:00', 30, 50000, 'Finalizado', 4, 3, NULL),
	(192, '2025-05-21', '14:59:00', 30, 50000, 'Finalizado', 4, 3, NULL),
	(193, '2025-05-21', '15:29:00', 30, 50000, 'Finalizado', 4, 3, NULL),
	(194, '2025-05-22', '10:59:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(195, '2025-05-22', '11:29:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(196, '2025-05-22', '11:59:00', 30, 50000, 'Reservado', 5, 3, 1),
	(197, '2025-05-22', '12:29:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(198, '2025-05-22', '12:59:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(199, '2025-05-22', '13:29:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(200, '2025-05-22', '13:59:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(201, '2025-05-22', '14:29:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(202, '2025-05-22', '14:59:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(203, '2025-05-22', '15:29:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(204, '2025-05-23', '10:59:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(205, '2025-05-23', '11:29:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(206, '2025-05-23', '11:59:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(207, '2025-05-23', '12:29:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(208, '2025-05-23', '12:59:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(209, '2025-05-23', '13:29:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(210, '2025-05-23', '13:59:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(211, '2025-05-23', '14:29:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(212, '2025-05-23', '14:59:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(213, '2025-05-23', '15:29:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(214, '2025-05-28', '10:59:00', 30, 50000, 'Disponible', 4, 3, NULL),
	(215, '2025-05-28', '11:29:00', 30, 50000, 'Disponible', 4, 3, NULL),
	(216, '2025-05-28', '11:59:00', 30, 50000, 'Disponible', 4, 3, NULL),
	(217, '2025-05-28', '12:29:00', 30, 50000, 'Disponible', 4, 3, NULL),
	(218, '2025-05-28', '12:59:00', 30, 50000, 'Disponible', 4, 3, NULL),
	(219, '2025-05-28', '13:29:00', 30, 50000, 'Disponible', 4, 3, NULL),
	(220, '2025-05-28', '13:59:00', 30, 50000, 'Disponible', 4, 3, NULL),
	(221, '2025-05-28', '14:29:00', 30, 50000, 'Disponible', 4, 3, NULL),
	(222, '2025-05-28', '14:59:00', 30, 50000, 'Disponible', 4, 3, NULL),
	(223, '2025-05-28', '15:29:00', 30, 50000, 'Disponible', 4, 3, NULL),
	(224, '2025-05-29', '10:59:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(225, '2025-05-29', '11:29:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(226, '2025-05-29', '11:59:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(227, '2025-05-29', '12:29:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(228, '2025-05-29', '12:59:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(229, '2025-05-29', '13:29:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(230, '2025-05-29', '13:59:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(231, '2025-05-29', '14:29:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(232, '2025-05-29', '14:59:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(233, '2025-05-29', '15:29:00', 30, 50000, 'Disponible', 5, 3, NULL),
	(234, '2025-05-30', '10:59:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(235, '2025-05-30', '11:29:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(236, '2025-05-30', '11:59:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(237, '2025-05-30', '12:29:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(238, '2025-05-30', '12:59:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(239, '2025-05-30', '13:29:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(240, '2025-05-30', '13:59:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(241, '2025-05-30', '14:29:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(242, '2025-05-30', '14:59:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(243, '2025-05-30', '15:29:00', 30, 50000, 'Disponible', 6, 3, NULL),
	(244, '2025-05-21', '12:00:00', 90, 30000, 'Finalizado', 4, 4, NULL),
	(245, '2025-05-21', '13:30:00', 90, 30000, 'Finalizado', 4, 4, NULL),
	(246, '2025-05-21', '15:00:00', 90, 30000, 'Finalizado', 4, 4, NULL),
	(247, '2025-05-21', '16:30:00', 90, 30000, 'Finalizado', 4, 4, NULL),
	(248, '2025-05-21', '18:00:00', 90, 30000, 'Disponible', 4, 4, NULL),
	(249, '2025-05-22', '12:00:00', 90, 30000, 'Disponible', 5, 4, NULL),
	(250, '2025-05-22', '13:30:00', 90, 30000, 'Disponible', 5, 4, NULL),
	(251, '2025-05-22', '15:00:00', 90, 30000, 'Disponible', 5, 4, NULL),
	(252, '2025-05-22', '16:30:00', 90, 30000, 'Disponible', 5, 4, NULL),
	(253, '2025-05-22', '18:00:00', 90, 30000, 'Disponible', 5, 4, NULL),
	(254, '2025-05-23', '12:00:00', 90, 30000, 'Disponible', 6, 4, NULL),
	(255, '2025-05-23', '13:30:00', 90, 30000, 'Disponible', 6, 4, NULL),
	(256, '2025-05-23', '15:00:00', 90, 30000, 'Disponible', 6, 4, NULL),
	(257, '2025-05-23', '16:30:00', 90, 30000, 'Disponible', 6, 4, NULL),
	(258, '2025-05-23', '18:00:00', 90, 30000, 'Disponible', 6, 4, NULL),
	(259, '2025-05-27', '12:00:00', 90, 30000, 'Disponible', 3, 4, NULL),
	(260, '2025-05-27', '13:30:00', 90, 30000, 'Disponible', 3, 4, NULL),
	(261, '2025-05-27', '15:00:00', 90, 30000, 'Disponible', 3, 4, NULL),
	(262, '2025-05-27', '16:30:00', 90, 30000, 'Disponible', 3, 4, NULL),
	(263, '2025-05-27', '18:00:00', 90, 30000, 'Disponible', 3, 4, NULL),
	(264, '2025-05-28', '12:00:00', 90, 30000, 'Disponible', 4, 4, NULL),
	(265, '2025-05-28', '13:30:00', 90, 30000, 'Disponible', 4, 4, NULL),
	(266, '2025-05-28', '15:00:00', 90, 30000, 'Disponible', 4, 4, NULL),
	(267, '2025-05-28', '16:30:00', 90, 30000, 'Disponible', 4, 4, NULL),
	(268, '2025-05-28', '18:00:00', 90, 30000, 'Disponible', 4, 4, NULL),
	(269, '2025-05-29', '12:00:00', 90, 30000, 'Disponible', 5, 4, NULL),
	(270, '2025-05-29', '13:30:00', 90, 30000, 'Disponible', 5, 4, NULL),
	(271, '2025-05-29', '15:00:00', 90, 30000, 'Disponible', 5, 4, NULL),
	(272, '2025-05-29', '16:30:00', 90, 30000, 'Disponible', 5, 4, NULL),
	(273, '2025-05-29', '18:00:00', 90, 30000, 'Disponible', 5, 4, NULL),
	(274, '2025-05-30', '12:00:00', 90, 30000, 'Disponible', 6, 4, NULL),
	(275, '2025-05-30', '13:30:00', 90, 30000, 'Disponible', 6, 4, NULL),
	(276, '2025-05-30', '15:00:00', 90, 30000, 'Disponible', 6, 4, NULL),
	(277, '2025-05-30', '16:30:00', 90, 30000, 'Disponible', 6, 4, NULL),
	(278, '2025-05-30', '18:00:00', 90, 30000, 'Disponible', 6, 4, NULL);

-- Volcando estructura para tabla reservas.informacion_empresa
CREATE TABLE IF NOT EXISTS `informacion_empresa` (
  `id_infoEmpresa` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `telefono` bigint NOT NULL,
  `correo` varchar(100) NOT NULL,
  `direccion` varchar(100) NOT NULL,
  `descripcion` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `fk_usuario` bigint NOT NULL,
  PRIMARY KEY (`id_infoEmpresa`),
  KEY `fk_usuario` (`fk_usuario`),
  CONSTRAINT `fk_usuario` FOREIGN KEY (`fk_usuario`) REFERENCES `usuario` (`numero_doc`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.informacion_empresa: ~0 rows (aproximadamente)
INSERT INTO `informacion_empresa` (`id_infoEmpresa`, `nombre`, `telefono`, `correo`, `direccion`, `descripcion`, `fk_usuario`) VALUES
	(1, 'Reserv+', 3245644567, 'reserplus@gmail.com', 'cra #7', 'Tu sistema gestor de confianza', 14234);

-- Volcando estructura para procedimiento reservas.info_empresa
DELIMITER //
CREATE PROCEDURE `info_empresa`()
BEGIN
	SELECT * FROM informacion_empresa
	ORDER BY informacion_empresa.id_infoEmpresa DESC
	LIMIT 1;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.mis_multas
DELIMITER //
CREATE PROCEDURE `mis_multas`(
	IN `p_id_usuario` BIGINT
)
BEGIN
	SELECT multa.Pk_multa, multa.fecha_multa, multa.fecha_fin, multa.costo, multa.descripcion, multa.estado, multa.PFK_reserva, multa.fk_tp_Multa
	FROM multa
	LEFT JOIN reserva
	ON multa.PFK_reserva = reserva.PK_reserva
	LEFT JOIN usuario
	ON reserva.fk_usuario_cliente = usuario.numero_doc
	WHERE usuario.numero_doc = p_id_usuario;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.mis_reservas
DELIMITER //
CREATE PROCEDURE `mis_reservas`(
	IN `p_id_usuario` BIGINT
)
BEGIN
SELECT reserva.PK_reserva AS ID, horario.fk_recurso, recurso.nombre AS 'recurso', reserva.fecha_registro, horario.PK_horario, horario.fecha, 
	horario.hora_inicio, horario.duracion, horario.costo, estado_reserva.nombre AS estado, estado_reserva.Pk_estado_reserva AS pk_estado
FROM usuario
	INNER JOIN reserva
		ON reserva.fk_usuario_cliente = usuario.numero_doc
	INNER JOIN estado_reserva
		ON reserva.fk_estado_reserva = estado_reserva.Pk_estado_reserva
	INNER JOIN horario
		ON horario.fk_reserva = reserva.PK_reserva
	INNER JOIN recurso
		ON horario.fk_recurso = recurso.PK_recurso
	WHERE reserva.fk_estado_reserva != 4 AND usuario.numero_doc = p_id_usuario;
END//
DELIMITER ;

-- Volcando estructura para tabla reservas.multa
CREATE TABLE IF NOT EXISTS `multa` (
  `Pk_multa` int NOT NULL AUTO_INCREMENT,
  `fecha_multa` date NOT NULL DEFAULT (curdate()),
  `fecha_fin` date DEFAULT NULL,
  `costo` float DEFAULT '0',
  `descripcion` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `estado` enum('Activa','Finalizada','Pagada') CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Activa',
  `PFK_reserva` int NOT NULL,
  `fk_tp_Multa` int NOT NULL,
  PRIMARY KEY (`Pk_multa`,`PFK_reserva`),
  UNIQUE KEY `Pk_multa_UNIQUE` (`Pk_multa`),
  KEY `fk_Multa_Reserva1_idx` (`PFK_reserva`),
  KEY `fk_Multa_Tipo_Multa1_idx` (`fk_tp_Multa`),
  CONSTRAINT `fk_Multa_Reserva1` FOREIGN KEY (`PFK_reserva`) REFERENCES `reserva` (`PK_reserva`),
  CONSTRAINT `fk_Multa_Tipo_Multa1` FOREIGN KEY (`fk_tp_Multa`) REFERENCES `tipo_multa` (`PK_tipo_multa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.multa: ~0 rows (aproximadamente)

-- Volcando estructura para procedimiento reservas.obtenerUsuario
DELIMITER //
CREATE PROCEDURE `obtenerUsuario`(
	IN `email` VARCHAR(50)
)
BEGIN
SELECT usuario.nombre, usuario.apellido, usuario.restablecer, usuario.token,  usuario.contrasenia
FROM usuario
WHERE usuario.correo = email;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.pagar_multa
DELIMITER //
CREATE PROCEDURE `pagar_multa`(
	IN `p_idmulta` INT
)
BEGIN
	UPDATE multa
	SET multa.estado = 'Finalizada'
	WHERE multa.Pk_multa = p_idmulta;
END//
DELIMITER ;

-- Volcando estructura para tabla reservas.permiso
CREATE TABLE IF NOT EXISTS `permiso` (
  `Pk_permiso` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `ESTADO_PERMISO` tinyint DEFAULT '0',
  PRIMARY KEY (`Pk_permiso`),
  UNIQUE KEY `Pk_permiso_UNIQUE` (`Pk_permiso`)
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.permiso: ~29 rows (aproximadamente)
INSERT INTO `permiso` (`Pk_permiso`, `nombre`, `ESTADO_PERMISO`) VALUES
	(1, 'Gestionar recurso', 0),
	(2, 'Crear recurso', 0),
	(3, 'Asignar agenda', 0),
	(4, 'Gestionar usuarios', 0),
	(5, 'Crear usuario', 0),
	(6, 'Gestionar multas', 0),
	(7, 'Mis multas', 0),
	(8, 'Gestion de roles', 0),
	(9, 'Reservas de los recursos', 0),
	(10, 'Realizar reserva', 0),
	(11, 'Ver reservas', 0),
	(12, 'Ver agenda', 0),
	(13, 'Informacion empresa', 0),
	(14, 'Reservas del recurso', 0),
	(15, 'Gestionar agendas', 0),
	(16, 'Editar usuarios', 0),
	(17, 'Crear multas', 0),
	(18, 'Editar multas', 0),
	(19, 'Eliminar multas', 0),
	(20, 'Parametrizar tipo de multas', 0),
	(21, 'Crear roles', 0),
	(22, 'Asignar permisos a los roles', 0),
	(23, 'Reservas de todos los recursos', 0),
	(24, 'Editar reserva', 0),
	(25, 'Reporte usuarios', 0),
	(26, 'Eliminar horario', 0),
	(27, 'Reporte agenda', 0),
	(28, 'Reporte recursos', 0),
	(29, 'Reporte reservas', 0);

-- Volcando estructura para tabla reservas.permiso_rol
CREATE TABLE IF NOT EXISTS `permiso_rol` (
  `PK_ID_PERMISO_ROL` int NOT NULL AUTO_INCREMENT,
  `PFK_idpermiso` int NOT NULL,
  `PFK_idrol` int NOT NULL,
  `ESTADO_PERMISO_ROL` tinyint NOT NULL,
  PRIMARY KEY (`PK_ID_PERMISO_ROL`,`PFK_idpermiso`,`PFK_idrol`) USING BTREE,
  KEY `fk_Permiso_has_Rol_Permiso1_idx` (`PFK_idpermiso`),
  KEY `fk_Permiso_has_Rol_Rol1_idx` (`PFK_idrol`) USING BTREE,
  CONSTRAINT `fk_Permiso_has_Rol_Permiso1` FOREIGN KEY (`PFK_idpermiso`) REFERENCES `permiso` (`Pk_permiso`),
  CONSTRAINT `fk_Permiso_has_Rol_Rol1` FOREIGN KEY (`PFK_idrol`) REFERENCES `rol` (`PK_rol`)
) ENGINE=InnoDB AUTO_INCREMENT=49 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.permiso_rol: ~46 rows (aproximadamente)
INSERT INTO `permiso_rol` (`PK_ID_PERMISO_ROL`, `PFK_idpermiso`, `PFK_idrol`, `ESTADO_PERMISO_ROL`) VALUES
	(1, 1, 1, 0),
	(2, 1, 3, 1),
	(3, 2, 3, 1),
	(4, 3, 3, 1),
	(5, 4, 3, 1),
	(6, 5, 3, 1),
	(7, 6, 3, 1),
	(8, 7, 3, 1),
	(9, 8, 3, 1),
	(10, 9, 3, 1),
	(11, 10, 3, 1),
	(12, 11, 3, 1),
	(13, 12, 3, 1),
	(14, 13, 3, 1),
	(15, 14, 3, 1),
	(16, 2, 1, 0),
	(17, 3, 1, 0),
	(18, 12, 1, 0),
	(19, 13, 1, 0),
	(20, 7, 1, 1),
	(21, 15, 3, 1),
	(24, 11, 1, 1),
	(25, 10, 1, 1),
	(26, 7, 2, 0),
	(27, 12, 2, 1),
	(28, 13, 2, 0),
	(29, 14, 2, 1),
	(30, 16, 3, 1),
	(31, 17, 3, 1),
	(32, 18, 3, 1),
	(33, 19, 3, 1),
	(34, 24, 3, 1),
	(35, 23, 3, 1),
	(36, 22, 3, 1),
	(37, 21, 3, 1),
	(38, 20, 3, 1),
	(39, 25, 3, 1),
	(40, 26, 3, 1),
	(41, 27, 3, 1),
	(42, 28, 3, 1),
	(43, 29, 3, 1),
	(44, 29, 2, 1),
	(45, 4, 1, 0),
	(46, 29, 1, 1),
	(47, 18, 1, 0),
	(48, 24, 1, 0);

-- Volcando estructura para tabla reservas.recurso
CREATE TABLE IF NOT EXISTS `recurso` (
  `PK_recurso` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(55) NOT NULL,
  `estado` enum('Activo','Inactivo') CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Activo',
  `Direccion` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FK_tp_recurso` int NOT NULL,
  `fk_usuario_encargado` bigint DEFAULT NULL,
  PRIMARY KEY (`PK_recurso`),
  UNIQUE KEY `PK_recurso_UNIQUE` (`PK_recurso`),
  KEY `fk_Recurso_Tipo_recurso1_idx` (`FK_tp_recurso`),
  KEY `fk_Recurso_Usuario1_idx` (`fk_usuario_encargado`),
  CONSTRAINT `fk_Recurso_Tipo_recurso1` FOREIGN KEY (`FK_tp_recurso`) REFERENCES `tipo_recurso` (`PK_tp_recurso`),
  CONSTRAINT `fk_Recurso_Usuario1` FOREIGN KEY (`fk_usuario_encargado`) REFERENCES `usuario` (`numero_doc`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.recurso: ~4 rows (aproximadamente)
INSERT INTO `recurso` (`PK_recurso`, `nombre`, `estado`, `Direccion`, `FK_tp_recurso`, `fk_usuario_encargado`) VALUES
	(1, 'Piscina grande', 'Activo', 'Cra #75', 1, 14234),
	(2, 'Barbería Xx', 'Activo', 'Cra #75', 1, 9813241),
	(3, 'Canchas pequeñas', 'Activo', 'Cra #75', 2, 239532),
	(4, 'Uñas', 'Activo', 'Cra #75', 2, 4241142);

-- Volcando estructura para tabla reservas.reserva
CREATE TABLE IF NOT EXISTS `reserva` (
  `PK_reserva` int NOT NULL AUTO_INCREMENT,
  `fecha_registro` date NOT NULL DEFAULT (curdate()),
  `fk_estado_reserva` int NOT NULL DEFAULT '2',
  `fk_usuario_cliente` bigint NOT NULL,
  PRIMARY KEY (`PK_reserva`),
  UNIQUE KEY `PK_reserva_UNIQUE` (`PK_reserva`),
  KEY `fk_Reserva_Estado_reserva1_idx` (`fk_estado_reserva`),
  KEY `fk_Reserva_Usuario1_idx` (`fk_usuario_cliente`),
  CONSTRAINT `fk_Reserva_Estado_reserva1` FOREIGN KEY (`fk_estado_reserva`) REFERENCES `estado_reserva` (`Pk_estado_reserva`),
  CONSTRAINT `fk_Reserva_Usuario1` FOREIGN KEY (`fk_usuario_cliente`) REFERENCES `usuario` (`numero_doc`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.reserva: ~1 rows (aproximadamente)
INSERT INTO `reserva` (`PK_reserva`, `fecha_registro`, `fk_estado_reserva`, `fk_usuario_cliente`) VALUES
	(1, '2025-05-21', 2, 14234);

-- Volcando estructura para vista reservas.reservas_view
-- Creando tabla temporal para superar errores de dependencia de VIEW
CREATE TABLE `reservas_view` (
	`recurso` VARCHAR(1) NOT NULL COLLATE 'utf8mb3_general_ci',
	`nombre` VARCHAR(1) NOT NULL COLLATE 'utf8mb3_general_ci',
	`apellido` VARCHAR(1) NOT NULL COLLATE 'utf8mb3_general_ci',
	`fecha_registro` DATE NOT NULL,
	`fecha` DATE NOT NULL,
	`hora_inicio` TIME NOT NULL,
	`duracion` INT NOT NULL,
	`costo` FLOAT NOT NULL,
	`estado` VARCHAR(1) NOT NULL COLLATE 'utf8mb3_general_ci',
	`PK_horario` INT NOT NULL,
	`PK_reserva` INT NOT NULL,
	`PK_recurso` INT NOT NULL
) ENGINE=MyISAM;

-- Volcando estructura para procedimiento reservas.restablecer_contrasenia
DELIMITER //
CREATE PROCEDURE `restablecer_contrasenia`(
	IN `restablecer` INT,
	IN `u_contrasenia` VARCHAR(50),
	IN `token` VARCHAR(200)
)
BEGIN
    UPDATE USUARIO
    SET usuario.restablecer = restablecer, usuario.contrasenia = u_contrasenia
    WHERE usuario.token  = token;
END//
DELIMITER ;

-- Volcando estructura para tabla reservas.rol
CREATE TABLE IF NOT EXISTS `rol` (
  `PK_rol` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `estado` enum('Activo','Inactivo') CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Activo',
  PRIMARY KEY (`PK_rol`),
  UNIQUE KEY `PK_rol_UNIQUE` (`PK_rol`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.rol: ~3 rows (aproximadamente)
INSERT INTO `rol` (`PK_rol`, `nombre`, `estado`) VALUES
	(1, 'Usuario', 'Activo'),
	(2, 'Encargado', 'Activo'),
	(3, 'Administrador', 'Activo');

-- Volcando estructura para procedimiento reservas.tiene_multa
DELIMITER //
CREATE PROCEDURE `tiene_multa`(
	IN `p_reserva` INT
)
BEGIN
	SELECT multa.Pk_multa
	FROM multa
	INNER JOIN reserva
	ON multa.PFK_reserva = reserva.PK_reserva
	WHERE reserva.PK_reserva = p_reserva
	LIMIT 1;
END//
DELIMITER ;

-- Volcando estructura para procedimiento reservas.tiene_recurso
DELIMITER //
CREATE PROCEDURE `tiene_recurso`(
	IN `p_id_usuario` BIGINT
)
BEGIN
	SELECT recurso.PK_recurso
	FROM recurso
	INNER JOIN usuario
	ON recurso.fk_usuario_encargado = usuario.numero_doc
	WHERE usuario.numero_doc = p_id_usuario
	LIMIT 1;
END//
DELIMITER ;

-- Volcando estructura para tabla reservas.tipo_documento
CREATE TABLE IF NOT EXISTS `tipo_documento` (
  `PK_tipo_doc` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(55) NOT NULL,
  `estado` enum('Activo','Inactivo') NOT NULL DEFAULT 'Activo',
  PRIMARY KEY (`PK_tipo_doc`),
  UNIQUE KEY `PK_tipo_doc_UNIQUE` (`PK_tipo_doc`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.tipo_documento: ~3 rows (aproximadamente)
INSERT INTO `tipo_documento` (`PK_tipo_doc`, `nombre`, `estado`) VALUES
	(1, 'Cedula', 'Activo'),
	(2, 'Tarjeta identidad', 'Activo'),
	(3, 'Pasaporte', 'Activo');

-- Volcando estructura para tabla reservas.tipo_multa
CREATE TABLE IF NOT EXISTS `tipo_multa` (
  `PK_tipo_multa` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `valor` float NOT NULL DEFAULT '0',
  `dias` int NOT NULL DEFAULT '0',
  `estado` enum('Activo','Inactivo') CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Activo',
  PRIMARY KEY (`PK_tipo_multa`),
  UNIQUE KEY `PK_tipo_multa_UNIQUE` (`PK_tipo_multa`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.tipo_multa: ~4 rows (aproximadamente)
INSERT INTO `tipo_multa` (`PK_tipo_multa`, `nombre`, `valor`, `dias`, `estado`) VALUES
	(1, 'Dias', 0, 5, 'Activo'),
	(2, 'Costo', 20000, 0, 'Activo'),
	(3, 'Dias y precio', 10000, 3, 'Activo'),
	(4, 'Multa total', 12000, 0, 'Activo');

-- Volcando estructura para tabla reservas.tipo_recurso
CREATE TABLE IF NOT EXISTS `tipo_recurso` (
  `PK_tp_recurso` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  PRIMARY KEY (`PK_tp_recurso`),
  UNIQUE KEY `PK_tp_recurso_UNIQUE` (`PK_tp_recurso`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.tipo_recurso: ~2 rows (aproximadamente)
INSERT INTO `tipo_recurso` (`PK_tp_recurso`, `nombre`) VALUES
	(1, 'Persona'),
	(2, 'Fisico');

-- Volcando estructura para tabla reservas.usuario
CREATE TABLE IF NOT EXISTS `usuario` (
  `numero_doc` bigint NOT NULL,
  `telefono` bigint NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `apellido` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `correo` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `contrasenia` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `token` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `fecha_registro` date NOT NULL DEFAULT (curdate()),
  `estado` enum('Activo','Inactivo','Suspendido') NOT NULL DEFAULT 'Activo',
  `Fk_tp_documento` int NOT NULL,
  `Fk_rol` int NOT NULL DEFAULT '1',
  `restablecer` bit(1) NOT NULL,
  PRIMARY KEY (`numero_doc`),
  UNIQUE KEY `numero_doc_UNIQUE` (`numero_doc`),
  UNIQUE KEY `correo` (`correo`),
  KEY `fk_Usuario_Tipo_documento1_idx` (`Fk_tp_documento`),
  KEY `fk_Usuario_Rol1_idx` (`Fk_rol`),
  CONSTRAINT `fk_Usuario_Rol1` FOREIGN KEY (`Fk_rol`) REFERENCES `rol` (`PK_rol`),
  CONSTRAINT `fk_Usuario_Tipo_documento1` FOREIGN KEY (`Fk_tp_documento`) REFERENCES `tipo_documento` (`PK_tipo_doc`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla reservas.usuario: ~8 rows (aproximadamente)
INSERT INTO `usuario` (`numero_doc`, `telefono`, `nombre`, `apellido`, `correo`, `contrasenia`, `token`, `fecha_registro`, `estado`, `Fk_tp_documento`, `Fk_rol`, `restablecer`) VALUES
	(14234, 323454, 'yan', 'villegas', 'yan@gmail.com', 'siu231', 'e5de7e5894d047fc9050a98ebd0f4152', '2023-11-14', 'Activo', 1, 3, b'0'),
	(239532, 3423423, 'Alberto', 'Rodriguez', 'albert@gmail.com', 'albert123', '2745ba403ffd475885578d7041a27bac', '2025-05-21', 'Activo', 1, 2, b'0'),
	(4241142, 3423423, 'Camila', 'Montes', 'camila@gmail.com', 'camila123', '8726a841ad87477db291f533a41fd3a7', '2025-05-21', 'Activo', 1, 2, b'0'),
	(7254323, 3423423, 'Juan', 'Sanchez', 'juan@gmail.com', 'juan123', 'fb98d17b5d79435dbefcecd579fc37a0', '2025-05-21', 'Activo', 1, 1, b'0'),
	(8245432, 3423423, 'Joan', 'Pinto', 'joan@gmail.com', 'joan123', 'a6811c9936944155a4450919476adf48', '2025-05-21', 'Activo', 1, 1, b'0'),
	(9813241, 423424432, 'Cesar', 'España', 'cesar@gmail.com', 'cesar123', '4567c7a6a2fa4af395b23fc8adcb5eb3', '2025-05-21', 'Activo', 1, 2, b'0'),
	(82423423, 3423423, 'Nicolas', 'Correa', 'nicolas@gmail.com', 'nicolas123', '3d7ab18d9bad4649b65dae76c029c1c6', '2025-05-21', 'Activo', 1, 1, b'0'),
	(424268680, 3423423, 'Maria', 'España', 'maria@gmail.com', 'maria123', 'ebef77b6c3c54938903bf3e265bcebb4', '2025-05-21', 'Activo', 1, 1, b'0');

-- Volcando estructura para procedimiento reservas.validar_inicio
DELIMITER //
CREATE PROCEDURE `validar_inicio`(
	IN `u_correo` VARCHAR(45),
	IN `u_contrasenia` VARCHAR(45)
)
BEGIN
	SELECT usuario.numero_doc, usuario.telefono, usuario.nombre, usuario.apellido, usuario.correo, usuario.contrasenia, usuario.fecha_registro, 
	usuario.estado, usuario.Fk_tp_documento, tipo_documento.nombre AS 'tp_document', usuario.Fk_rol, rol.nombre AS 'rol'
	FROM usuario
	INNER JOIN tipo_documento
	ON usuario.Fk_tp_documento = tipo_documento.PK_tipo_doc
	INNER JOIN rol
	ON usuario.Fk_rol = rol.PK_rol
	WHERE usuario.correo =  u_correo AND usuario.contrasenia = u_contrasenia  AND usuario.estado != 'Inactivo';
END//
DELIMITER ;

-- Volcando estructura para disparador reservas.estadoU_finMulta
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `estadoU_finMulta` BEFORE UPDATE ON `multa` FOR EACH ROW BEGIN
	IF NEW.estado <> 'Activa' THEN
		UPDATE usuario
		INNER JOIN reserva
		ON reserva.fk_usuario_cliente = usuario.numero_doc
		INNER JOIN multa
		ON multa.PFK_reserva = reserva.PK_reserva
		SET usuario.estado = 'Activo'
		WHERE multa.Pk_multa = NEW.Pk_multa;
	END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Volcando estructura para disparador reservas.multa_actualizada
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `multa_actualizada` BEFORE UPDATE ON `multa` FOR EACH ROW BEGIN
	DECLARE dias_multa INT;
	DECLARE monto FLOAT;
	IF NEW.fk_tp_Multa <> OLD.fk_tp_Multa THEN
		SELECT tipo_multa.dias INTO dias_multa
	   	FROM tipo_multa
	   	WHERE tipo_multa.PK_tipo_multa = NEW.fk_tp_Multa;
	   	
	   SELECT tipo_multa.valor INTO monto
	   	FROM tipo_multa
	   	WHERE tipo_multa.PK_tipo_multa = NEW.fk_tp_Multa;
		
		IF dias_multa != 0 THEN
			SET NEW.fecha_fin = DATE_ADD(NEW.fecha_multa, INTERVAL dias_multa DAY);
		ELSEIF monto != 0 THEN
			SET NEW.fecha_fin = NULL;
		END IF;
		SET NEW.costo = monto;
	END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Volcando estructura para disparador reservas.registro_auditoria
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `registro_auditoria` AFTER INSERT ON `informacion_empresa` FOR EACH ROW BEGIN
	DECLARE new_data JSON;
	SET new_data = JSON_OBJECT('id_infoEmpresa', NEW.id_infoEmpresa, 'nombre', NEW.nombre, 'telefono', NEW.telefono, 'correo', NEW.correo,
	'direccion', NEW.direccion, 'descripcion', NEW.descripcion);
	INSERT INTO auditoria_empresa (registro_nuevo, usuario)
	VALUES (new_data, NEW.fk_usuario);
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Volcando estructura para disparador reservas.reserva_cancelada
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `reserva_cancelada` BEFORE DELETE ON `reserva` FOR EACH ROW BEGIN
	UPDATE horario
	SET horario.estado = 'Disponible', horario.fk_reserva = null
	WHERE horario.fk_reserva = OLD.PK_reserva;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Volcando estructura para disparador reservas.validar_horario
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `validar_horario` BEFORE INSERT ON `horario` FOR EACH ROW BEGIN
	IF EXISTS (
		SELECT 1
		FROM horario
		WHERE horario.fk_recurso = NEW.fk_recurso AND horario.fecha = NEW.fecha AND horario.hora_inicio = NEW.hora_inicio
	)THEN
		SIGNAL SQLSTATE '45000'
	        SET MESSAGE_TEXT = 'No se puede insertar el horario. Ya existe un registro con la misma fecha y hora.';
	END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Volcando estructura para disparador reservas.validar_multa
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `validar_multa` BEFORE INSERT ON `multa` FOR EACH ROW BEGIN
	DECLARE dias_multa INT;
	DECLARE id_user INT;
	DECLARE monto FLOAT;
	IF NOT EXISTS (
		SELECT 1
		FROM multa
			WHERE multa.PFK_reserva = NEW.PFK_reserva
	) THEN
		SELECT tipo_multa.dias INTO dias_multa
	   	FROM tipo_multa
	   	WHERE tipo_multa.PK_tipo_multa = NEW.fk_tp_Multa;
	   
	   SELECT tipo_multa.valor INTO monto
	   	FROM tipo_multa
	   	WHERE tipo_multa.PK_tipo_multa = NEW.fk_tp_Multa;
		
		IF dias_multa != 0 THEN
			SET NEW.fecha_fin = DATE_ADD(NEW.fecha_multa, INTERVAL dias_multa DAY);
		END IF;
		
		SET NEW.costo = monto;
		
		SELECT u.numero_doc
		INTO id_user
		FROM usuario u
		INNER JOIN reserva r ON r.fk_usuario_cliente = u.numero_doc
			WHERE r.PK_reserva = NEW.PFK_reserva
		LIMIT 1;
		
		UPDATE usuario
			SET usuario.estado = 'Suspendido'
			WHERE usuario.numero_doc = id_user;
		
	ELSE
		SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La reserva ya tiene una multa asignada';
	END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Eliminando tabla temporal y crear estructura final de VIEW
DROP TABLE IF EXISTS `reservas_view`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `reservas_view` AS select `recurso`.`nombre` AS `recurso`,`usuario`.`nombre` AS `nombre`,`usuario`.`apellido` AS `apellido`,`reserva`.`fecha_registro` AS `fecha_registro`,`horario`.`fecha` AS `fecha`,`horario`.`hora_inicio` AS `hora_inicio`,`horario`.`duracion` AS `duracion`,`horario`.`costo` AS `costo`,`estado_reserva`.`nombre` AS `estado`,`horario`.`PK_horario` AS `PK_horario`,`reserva`.`PK_reserva` AS `PK_reserva`,`recurso`.`PK_recurso` AS `PK_recurso` from ((((`recurso` join `horario` on((`horario`.`fk_recurso` = `recurso`.`PK_recurso`))) join `reserva` on((`horario`.`fk_reserva` = `reserva`.`PK_reserva`))) join `usuario` on((`reserva`.`fk_usuario_cliente` = `usuario`.`numero_doc`))) join `estado_reserva` on((`reserva`.`fk_estado_reserva` = `estado_reserva`.`Pk_estado_reserva`))) order by `horario`.`fecha`
;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
