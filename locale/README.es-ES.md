<p align="center">
	<img width="256px" src="https://github.com/GodotParadise/Achievements-CSharp/blob/main/icon.jpg" alt="GodotParadiseAchievements logo" />
	<h1 align="center">Godot Paradise Achievements</h1>
	
[![LastCommit](https://img.shields.io/github/last-commit/GodotParadise/Achievements?cacheSeconds=600)](https://github.com/GodotParadise/Achievements-CSharp/commits)
[![Stars](https://img.shields.io/github/stars/godotparadise/Achievements)](https://github.com/GodotParadise/Achievements-CSharp/stargazers)
[![Total downloads](https://img.shields.io/github/downloads/GodotParadise/Achievements-CSharp/total.svg?label=Downloads&logo=github&cacheSeconds=600)](https://github.com/GodotParadise/Achievements-CSharp/releases)
[![License](https://img.shields.io/github/license/GodotParadise/Achievements?cacheSeconds=2592000)](https://github.com/GodotParadise/Achievements-CSharp/blob/main/LICENSE.md)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat&logo=github)](https://github.com/godotparadise/Achievements-CSharp/pulls)
[![](https://img.shields.io/discord/1167079890391138406.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/XqS7C34x)
[![Kofi](https://badgen.net/badge/icon/kofi?icon=kofi&label)](https://ko-fi.com/bananaholograma)
</p>

[![en](https://img.shields.io/badge/lang-en-red.svg)](https://github.com/GodotParadise/Achievements-CSharp/blob/main/README.md)

- - -

Implementa un sistema de logros en tu juego de forma sencilla y con unas prácticas de seguridad mínimas.
- [Requerimientos](#requerimientos)
- [✨Instalacion](#instalacion)
  - [Automatica (Recomendada)](#automatica-recomendada)
  - [Manual](#manual)
  - [GDScript version](#gdscript-version)
- [Como empezar](#como-empezar)
- [Ready](#ready)
- [Estructura del archivo de logros](#estructura-del-archivo-de-logros)
- [Variables accessibles](#variables-accessibles)
- [Funciones](#funciones)
  - [Dictionary GetAchievement(string name)](#dictionary-getachievementstring-name)
  - [UpdateAchievement(string name, Dictionary data)](#updateachievementstring-name-dictionary-data)
  - [UnlockAchievement(string name)](#unlockachievementstring-name)
  - [ResetAchievement(string name, Dictionary data)](#resetachievementstring-name-dictionary-data)
- [Señales](#señales)
- [✌️Eres bienvenido a](#️eres-bienvenido-a)
- [🤝Normas de contribución](#normas-de-contribución)
- [📇Contáctanos](#contáctanos)


# Requerimientos
📢 No ofrecemos soporte para Godot 3+ ya que nos enfocamos en las versiones futuras estables a partir de la versión 4.
* Godot 4+

# ✨Instalacion
## Automatica (Recomendada)
Puedes descargar este plugin desde la [Godot asset library](https://godotengine.org/asset-library/asset/2039) oficial usando la pestaña AssetLib de tu editor Godot. Una vez instalado, estás listo para empezar
## Manual 
Para instalar manualmente el plugin, crea una carpeta **"addons"** en la raíz de tu proyecto Godot y luego descarga el contenido de la carpeta **"addons"** de este repositorio
## GDScript version
Este plugin también ha sido escrito en GDScript, puedes encontrarlo en [Achievements](https://github.com/GodotParadise/Achievements)

# Como empezar
Puedes acceder a esta funcionalidad usando la clase `GodotParadiseAchievements` donde puedes interactuar con tu archivo fuente.

Antes de empezar es necesario establecer algunos ajustes del proyecto que estarán disponibles después de cargar el plugin:
![achievements-config](https://github.com/GodotParadise/Achievements-CSharp/blob/main/images/achievements_config.png)

**Local source** se refiere a la ruta del archivo local que contiene la plantilla de logros. Este archivo es de sólo lectura y se utiliza únicamente para definir la estructura de los logros en su juego. Por ejemplo: `res://settings/achievements.json`

**Remote source** por otro lado, se refiere a la ruta del archivo JSON remoto que también contiene la plantilla de logros. Se aplican las mismas reglas que para la fuente local, pero esta información se obtiene de una URL remota. Por ejemplo: `https://myserver/achievements.json`

**Save directory** es la ubicación en la que se creará en la máquina del jugador el archivo guardado encriptado, utilizado para realizar un seguimiento del progreso de los logros. Por defecto, utiliza `OS.get_user_data_dir()/[project_name]`

**Save file name** es el nombre del archivo encriptado que registra el progreso de los logros. Por defecto, se llama `achievements.json`

**Password** es el conjunto de caracteres utilizado para cifrar y descifrar el archivo de logros guardado. Por defecto, genera una cadena aleatoria con una longitud de 25 caracteres. Esta longitud debería ser suficiente para la mayoría de los casos de uso, garantizando que los jugadores no puedan alterar su progreso de logros accediendo al archivo.

# Ready
Cuando este nodo está listo y entra en el arbol de escenas, realiza varias acciones:
1. Se conecta a la señal `AchievementUpdated`, que actualiza el archivo cifrado y comprueba si se han desbloqueado todos los logros. Si todos los logros están desbloqueados, emite la señal `AllAchievementsUnlocked`.
2. Crea el directorio de guardado utilizando la ruta definida en **ProjectSettings.**
3. Prepara los logros dentro de la clase leyendo de las fuentes definidas en **ProjectSettings.**
4. Sincroniza la última actualización de logros desde el archivo guardado encriptado si existe.

# Estructura del archivo de logros
El archivo JSON **debe seguir una estructura específica** para funcionar correctamente. Aunque puedes incluir propiedades personalizadas adicionales adaptadas a tu juego, hay algunas obligatorias que deben estar presentes:
```json
{
    "achievement-name": {
        "name": "MY achievement",
        "description": "Kill 25 enemies",
        "is_secret": false,
        "count_goal": 25,
        "current_progress": 0.0,
        "icon_path": "res://assets/icon/my-achievement.png",
        "unlocked": false,
        "active": true
    }
}
```
Es importante tener en cuenta que no todos los logros tendrán un requisito de `count_goal` para desbloquear el progreso. En los casos en los que este requisito no sea aplicable, debes dejar el valor de `count_goal` en cero. **La lógica y las condiciones para desbloquear los logros están totalmente determinadas por tu proyecto de juego.
Esta clase sirve de ayuda para actualizar y desbloquear logros mientras emite las señales apropiadas para la interacción.

# Variables accessibles
- Dictionary CurrentAchievements
- Dictionary UnlockedAchievements
- List<string> AchievementKeys
- 
# Funciones
## Dictionary GetAchievement(string name)
Recupera la información del logro deseado, si el nombre no existe como clave devolverá un diccionario vacío.

`GodotEssentialsAchievements.GetAchievement("orcs_party")`

## UpdateAchievement(string name, Dictionary data)
Esta función actualiza las propiedades del logro seleccionado, con valores del diccionario de datos que sustituyen a los existentes. Esta acción también emite la señal `AchievementUpdated`

`GodotEssentialsAchievements.UpdateAchievement("orcs_party", {"current_progress": 0.55})`

## UnlockAchievement(string name)
Si el logro no estaba desbloqueado previamente, esta función cambia la variable `unlocked` a true y emite la señal `AchievementUnlocked`. Esta acción desbloquea directamente el logro sin más comprobaciones.

`GodotEssentialsAchievements.AchievementUnlocked("orcs_party")`

## ResetAchievement(string name, Dictionary data)
Restablece el logro a un estado anterior. Los valores `current_progress` y `unlocked` se pondrán a 0 y false respectivamente. Puedes pasar como segundo parámetro los datos que quieras actualizar en este proceso.
Esta acción también emite las señales `AchievementReset` y `AchievementUpdated`.

`GodotEssentialsAchievements.ResetAchievement("orcs_party", {"description": "An orc party was discovered"})`

# Señales
- *AchievementUnlocked(string name, Dictionary achievement)*
- *AchievementUpdated(string name, Dictionary achievement)* 
- *AchievementReset(string name, Dictionary achievement)*
- *AllAchievementsUnlocked*


# ✌️Eres bienvenido a
- [Give feedback](https://github.com/GodotParadise/Achievements-CSharp/pulls)
- [Suggest improvements](https://github.com/GodotParadise/Achievements-CSharp/issues/new?assignees=BananaHolograma&labels=enhancement&template=feature_request.md&title=)
- [Bug report](https://github.com/GodotParadise/Achievements-CSharp/issues/new?assignees=BananaHolograma&labels=bug%2C+task&template=bug_report.md&title=)

GodotParadise esta disponible de forma gratuita.

Si estas agradecido por lo que hacemos, por favor, considera hacer una donación. Desarrollar los plugins y contenidos de GodotParadise requiere una gran cantidad de tiempo y conocimiento, especialmente cuando se trata de Godot. Incluso 1€ es muy apreciado y demuestra que te importa. ¡Muchas Gracias!

- - -
# 🤝Normas de contribución
**¡Gracias por tu interes en GodotParadise!**

Para garantizar un proceso de contribución fluido y colaborativo, revise nuestras [directrices de contribución](https://github.com/godotparadise/Achievements-CSharp/blob/main/CONTRIBUTING.md) antes de empezar. Estas directrices describen las normas y expectativas que mantenemos en este proyecto.

**📓Código de conducta:** En este proyecto nos adherimos estrictamente al [Código de conducta de Godot](https://godotengine.org/code-of-conduct/). Como colaborador, es importante respetar y seguir este código para mantener una comunidad positiva e inclusiva.
- - -


# 📇Contáctanos
Si has construido un proyecto, demo, script o algun otro ejemplo usando nuestros plugins haznoslo saber y podemos publicarlo en este repositorio para ayudarnos a mejorar y saber que lo que hacemos es útil.
