# Proyecto_WEB
Sistema de gestión de reservas, profecto final Gestión del conocimiento.

## Librearias usadas 

* AngleSharp
* ClosedXML
* Itext
* Newtonsoft.Json
* System.Data.SqlClient
* System.Text.Json


## Usuario

Las credenciales del usuario administrador del sistema son: 
* **Correo: yan@gmail.com**
* **Contraseña: siu231**


## Base de datos

Para el desarrollo de este proyecto se desarrollo una base de datos en MySQL llamada "reservas" que cuenta con 14 tablas.

**Tablas:** 
* dia: Guarda los nombres de los 7 dias de la semana.
* estado_reserva: Una tabla diseñada para que guarde el nombre del estado de una reserva.
* horario: Tabla que guarda el resultante de la paramtrización de la agenda.
* informacion_empresa: Guarda la información actual de la empresa.
* multa: Tabla que guarda las multas que se pueden otorgar a los usuario por inclumplir normas o politicas de la empresa.
* permiso: Los permisos que tendran los usuarios.
* permiso_rol: Relación entre los roles y los permisos que permite saber cuales son los permisos que tiene asignado un rol.
* recurso: Tabla que guarda los recursos.
* reserva: Tabla que guarda las reservas que han realizado los usuarios.
* rol: Tabla que guarda los roles que pueden tener los usuarios.
* tipo_documento: Tipo de documento de identidad de los usuarios.
* tipo_multa: Tabla que guarda la parametrización para las multas que puedan ser asignadas a los usuarios.
* tipo_recurso
* usuario: Tabla que guarda la información de los usuarios del sistema.



Para acceder a la base detatos se debe seguir la siguiente ruta: **AccesoDatos/Conexion**. En esta clase se encontrarán la logica y metodos para extraer los datos desde la BD.

**La cadena de conexión se podrá encontrar en esta clase en el metodo Conectar.**


Metodos de la clase conexión:
* Conectar: metodo que me permite conectarme a la base de datos.
* Desconectar: metodo que me permite desconectarme de la base detos una vez haya realizado las consultas o transacciones.
* Ejecutar consulta: metodo que me permite hacer una consulta a la base de datos a partir de unos parametros.
* Ejecutar transacción: metodo que me permite ejecutar transacciones ya sea de INSERT, UPDATE o DELETE con la base de datos.
* Ejecutar transacciones: metodo que me permite ejecutar muchas transacciones con la base de datos en caso de que la cantidad de información sea extensa.


## Procedimientos almacenados

La base de datos cuenta con 33 procedimientos almacenados, 5 triggers, 2 eventos.
