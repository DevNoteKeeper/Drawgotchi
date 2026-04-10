## Project: Drawgotchi

This project is inspired from the Tamagotchi game, people feed and put a creature to sleep using drawings!

## Rule of Drawgotchi

### State System

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

### Drawing Interaction

Visitor draw on a tablet. The drawing is classified by a CNN model into one of 6 classes and triggers a corresponding creature reaction.

**Class**
Vegetable, Chicken, Noodle, Bread, Drink, Bed

**Screen Example:**
![Nintendo](./Nintendo-DSi-Bl-Open.png)

- Clear button:  
  Erases the current drawing
- Send button:  
  submits drawing
  - On successful classification -> triggers creature reaction
  - On failed classification -> show subtitle (Animal Crossing style) such as "What's that? Ewww..."

**Interaction failure rules:**

- If model can't classify the drawing -> show subtitle
- When there is no submit in meal time or sleep time, show subtitle

### Hunger System

| Condition                           | Effect                                           |
| ----------------------------------- | ------------------------------------------------ |
| Hunger = 100 (starving)             | Happiness decreases faster                       |
| Hunger = 0 (full) and still feeding | Happiness -10, Energy -10 per feed (overfeeding) |

**Hunger effect of food(random by reset):**
| Class | Min| Max|
| -------- | ---| ---|
| Vegetable| 5 | 25 |
| Meat | 20 | 50 |
| Noodle | 15 | 40 |
| Bread | 10 | 35 |
| Drink | 5 | 20 |

**Like / Dislike:**  
 set 2 like, 2 dislike, 1 is middle.  
 Happiness recover of food(random by reset):
| Class | Min | Max |
| ---------- | ------- | ------- |
| Like (2) | 25 | 40 |
| Dislike (2) | -30 | -15 |
| Neutrality (1)| 0 | 15 |

### Sleeping System

| Condition            | Effect                                                  |
| -------------------- | ------------------------------------------------------- |
| Energy > 0           | Normal state, all interactions available                |
| Energy < 0           | Hunger/Happiness can't increase (feeding has no effect) |
| Energy = -100        | Forced sleep begins                                     |
| After 9pm (no sleep) | Energy decreases faster                                 |

- Drawing bed or Energy = -100 => Sleep starts
- During sleep => Hunger increase and Happiness decay slowed by 50%
- On wake up => Energy restored to 100, Happiness +30 bonus

### Reset System

when creature dies or is manually reset,

- all of the Food values are randomly reassigned for the new creature.
- 2 food classes assigned as Like, 2 as Dislike, 1 as Neutral. (Visitor discover preferences by feeding the creature)

### Death System

- If Hunger stays at 100 for 1m
- If Happiness stays at 0 for 1m
- On death: death animation plays
- After animation: automatically resets to Egg stage, new random values assigned

### Growth State

| State    | Condition                              | Effect                                 |
| -------- | -------------------------------------- | -------------------------------------- |
| Egg      | Start                                  | Visitor prompted to name the creature  |
| Hatching | Name is entered                        | Hatching animation plays automatically |
| Baby     | After hatching (Tutorial stage)        | No like/dislike, experience Day 1      |
| Adult    | Day 2+ AND avg(Hunger, Happiness) > 50 | like/dislike activates                 |

### Time Settings

- 1 day(in game) = 15 minutes(real-time)
- 7am start morning
- 7am set as breakfast, 12pm as lunch, 6pm as dinner => during 1m(in real time)
  - During each meal time (1 min real-time): Hunger increase rate \*2
- 9pm trigger: Energy decay accelerates

| Setting             | Value                                                |
| ------------------- | ---------------------------------------------------- |
| Hunger decay        | +2.5 per minutes                                     |
| Happiness decay     | -2.5 per minutes                                     |
| Energy decay        | -2.5 per minutes(normal), -12 per minutes(after 9pm) |
| Sleep duration      | 2-3 minutes (wakes at 7am = next day)                |
| Death threshold     | 1m at Hunger = 100 OR Happiness = 0                  |
| Expected full cycle | 45minutes is 1 cycle (birth to death)                |
