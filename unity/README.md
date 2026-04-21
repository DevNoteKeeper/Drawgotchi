# Unity Module (Game Engine)

Nintendo DS-style virtual pet game with drawing tablet input.

## Gameplay

Draw food or bed -> CNN classifies -> creature reacts with stats changes.

## Core Systems

### Growth Stages

| State    | Condition                              | Effect                                 |
| -------- | -------------------------------------- | -------------------------------------- |
| Egg      | Start                                  | Visitor prompted to name the creature  |
| Hatching | Name is entered                        | Hatching animation plays automatically |
| Baby     | After hatching (Tutorial stage)        | No like/dislike, experience Day 1      |
| Adult    | Day 2+ AND avg(Hunger, Happiness) > 50 | like/dislike activates                 |

### Stats

- **Hunger:**  
   range(0-100)  
   increase over time (0=Full, 100=starving)
- **Happiness:**  
   range(0-100)  
   decrease over time
- **Energy:**  
   range(-100-100)  
   decrease over time  
   after 9pm (in game time), decrease faster (-12/m)
- **Age:**  
   range(0-inf)  
   increase until death/reset

### Food Preferences (Adult level only)

set 2 like, 2 dislike, 1 is middle.  
 Happiness recover of food(random by reset):
| Class | Min | Max |
| ---------- | ------- | ------- |
| Like (2) | 25 | 40 |
| Dislike (2) | -30 | -15 |
| Neutrality (1)| 0 | 15 |

### Sleep & Death

- **Sleep**: Draw bed or Energy <= -100 => restores Energy to 100
- **Death**: Hunger=100 or Happiness=0 for 1 min => move to Death Scene
- **Over Feed**: Hunger=0 and feed => Happiness/Energy -10

## Controls

- **A**: Send drawing
- **X**: Clear canvas
- **Y**: Stats popup (shows level, preferences and stats)
- **Reset**: New creature
