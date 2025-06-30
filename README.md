<h1>Despliegue del Proyecto</h1>
1. Descargar en Proyecto del Repositorio
2. Guardarlo en una carpeta de acceso rapido
3. Descomprimir el archivo ZIP
4. Abrir el Archivo y Ejecutar "Inventario.sln"
5. Una vez dentro del proyecto dirigirse al archivo .json y cambiar la siguiente parte por el nombre de su computador:
 "ConnectionStrings": {
   "DefaultConnection": "Server=NOMBRE_DE_SU_COMPUTADOR;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;"
 }
 Nota: En Database se puede cambiar el nombre, ya que es el nombre de la base de datos que se quiere crear
 6. Diregirse a Herramientas > Administrador de Nuggets
 7. Abrir la Consola del Administrador de Paquetes
 8. Escribir el comando Add-Migration Initial, esto para que se realice la migracion de la base de datos
 9. Escribir el comando Update-Database, esto para una conexión correcta con la Base de Datos en SQL Server
 10. Con dichos cambios ya deberia iniciar el programa sin problemas.
