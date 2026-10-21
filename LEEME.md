# Inti y Killa · Avance jugable (Niveles 1, 2 y 3)

Grupo: Liliana Sibri · Gabriel Taipe · Santiago Camacho

## Cómo abrirlo en Unity
1. En Unity Hub crea un proyecto nuevo con la plantilla **2D** (sirve "2D (Built-In Render Pipeline)" o "Universal 2D"). Unity 2022.3 o Unity 6.
2. Copia la carpeta `IntiYKilla` dentro de la carpeta `Assets` del proyecto.
3. Al terminar de compilar, Unity crea solas las escenas **editables** `Assets/Scenes/IntiYKilla_Nivel1`, `Nivel2` y `Nivel3`,
   con todos los objetos del nivel (suelo, flores, enredaderas, enemigos, Sisa, la llama, la cámara...) y el arte guardado
   como imágenes PNG en `Assets/IntiYKilla/Art`. Puedes mover, copiar o borrar objetos y cambiar sus valores en el Inspector.
   Menú **Inti y Killa > Abrir Nivel 1 / 2 / 3** para abrir cada una.
   (**Inti y Killa > Generar escenas editables** las vuelve a crear desde el código y borra los cambios hechos a mano).
4. Abre una escena y dale **Play**.
5. Para generar el .exe: menú **Inti y Killa > Compilar ejecutable para Windows**. Queda en `Builds/IntiYKilla/IntiYKilla.exe`.

## Controles
- A / D o flechas: moverse
- Espacio / W: saltar (mantén para saltar más alto)
- Q: cambiar entre día (Inti) y noche (Killa)
- W / flecha arriba junto a una enredadera: trepar (Espacio para soltarse)
- En la pantalla de título: Enter empieza en el Nivel 1; 2 o 3 van directo a la Laguna o al Bosque
- E: recoger el pedazo del Intihuatana
- F (o J): lanzar una piedra con la huaraca (honda andina) para eliminar sombras, espíritus y guardianes
- Saltar encima de un enemigo también lo elimina
- R: volver al último punto de control · Esc: salir
- También funciona con control (stick, A saltar, Y o RB cambiar, X interactuar)

## Qué incluye cada nivel
**Nivel 1 · El Pueblo**
- El mundo empieza de noche, con una **sombra del Supay** rondando. La luz de Inti (Q) la disuelve.
- **Flores de kantuta**: de día se abren y sirven de plataformas; de noche se cierran.
- **Camino de luna**: piedras que Killa revela solo de noche, vigiladas por un **espíritu** que vuela.
- Puzzle final: subir a una piedra de luna, saltar y cambiar al día en el aire para que se abra la flor.

**Nivel 2 · La Laguna**
- De día el agua es profunda (si caes, reapareces); de noche **se congela** y se puede cruzar.
- Una **sombra patina sobre el hielo** de noche, y un **espíritu** vuela bajo sobre la segunda laguna.
- Puzzle (el ejemplo del documento): desde el hielo, saltar y cambiar al día en el aire para caer sobre la flor y llegar a la roca con el pedazo.

**Nivel 3 · El Bosque**
- **Enredaderas** que crecen de día y se pueden trepar; de noche se secan.
- **Ramas de luna**: caminos ocultos entre los árboles que solo existen de noche.
- Enemigos: **sombras veloces**, **espíritus** y un **guardián del bosque** nuevo que persigue a Sisa de noche.
- Puzzle final: subir a una rama de luna, saltar hacia la enredadera y cambiar al día en el aire para que crezca.

**En todos los niveles**
- **Pedazo del Intihuatana** (E) que abre la puerta de piedra; al terminar pasas al siguiente nivel.
- **Apachetas** como puntos de control; la **llama** acompaña a Sisa; **Inti y Killa** dan pistas.
- Destello, temblor y sonido al cambiar entre día y noche; pétalos de día y luciérnagas de noche.

## Sobre el arte
Todo el arte es **provisional (boceto en pixel art)**, igual que los sonidos, y se dibuja por código en `Scripts/SpriteFactory.cs`,
así el proyecto no depende de imágenes externas. En pantalla se indica "Prototipo · arte provisional".
Para el arte final se reemplazan esos sprites por ilustraciones.

## Estructura del código
- `Bootstrap.cs`: diseño de los tres niveles en código; con él se generan las escenas editables.
- `SlingStone.cs`: la piedra de la huaraca. `LevelInfo.cs`: qué nivel es cada escena.
- `PlayerController.cs`: movimiento de Sisa. `DayNightManager.cs`: la mecánica de día/noche.
- `FlowerPlatform.cs`, `MoonStone.cs`, `LakeTile.cs`, `Vine.cs`, `Enemy.cs`: elementos que cambian con el día y la noche.
- `GameManager.cs`: pantallas de título, interfaz y fin del nivel.
- `Editor/BuildTools.cs`: crea la escena y compila el ejecutable.
