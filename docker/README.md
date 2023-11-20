# Proyecto Docker para AIDate API

Este proyecto Docker está diseñado para crear un contenedor de desarrollo para la AIDate API.

## Instrucciones

1. Asegúrate de tener Docker instalado en tu sistema.

2. Clona este repositorio y navega a la carpeta `docker`.

3. Coloca el ejecutable APICore.exe y otros archivos necesarios en `net6.0/linux-x64/`.

4. Ejecuta el siguiente comando para construir y ejecutar el contenedor:

   ```bash
   docker-compose -f dev.yml up
