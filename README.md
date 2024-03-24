# CodeName.EventSystem

The event system works off of the following equations:
- State + Input = Next State
  - Applying input can give you the next state.
- State + Input = Event Log
  - Applying input can give you an event log.
- State + Event Log = Next State
    - Applying the event log can also give you the next state.
    - This allows for replays.

## GenerativeGameStateTracker

This tracker implements the equations:
- State + Input = Next State
- State + Input = Event Log

This is typically used on the server to generate an event log that can be sent to clients.

## RegenerativeGameStateTracker

This tracker implements the equation:
- State + Event Log = Next State

This is typically used on the client to play animations.

## ConstGameStateTracker

This tracker implements the equation:
- State + Const Event = State
  - This is not part of the original equations that the event system works off of.

This is typically used for querying the game's state.

### Const Events

Const events are events that are used with the ConstGameStateTracker.
Contrary to their name, the events themselves can be modified, but the GameState CANNOT BE MODIFIED.

## FAQ

### 1. When are events required?
1. Non-deterministic branching (run RNG, then save RNG result to event, don't use RNG result directly)
   - Events are used to snapshot the randomly generated value so that the same value can be used when replaying the event log.
2. When event handlers need to react to events that are happening.

### 2. Common desync issues
1. Duplicated entities -> Fix by using entity ids instead.
   - Serialized entities are duplicated and assigned a new ID
2. Incorrect non-deterministic branching. See previous FAQ.

### 3. Other desync issues
1. Floating point inaccuracy
   - Can't fix, but should be very rare; use integers or fixed point math if this is an issue

### Using the event matcher (need to verify syntax)
1. The following are equivalent, but MatchOn allows for more advanced queries (if needed)
   - `if (tracker.MatchOn<NameOfEvent, GameState>(out var result)) {}`
   - `if (tracker.CurrentEvent is NameOfEvent) {}`
   - `if (eventNode.Event is NameOfEvent) {}` (This one may get removed)

## Planned changes
1. Event Tree and List will be removed and replaced with an Event Stack
   - This should make logic more consistent when matching on events that are preceded by other events
     - Eg: Dodged an attack this turn vs Dodged an attack in the last X turns
     - With the current implementation, the Event List is used, but the Event List only tracks events in the current turn. This means checking for dodges in the last X turns has to be implemented in a different way.
   - Recommended migration:
     - Track whether a unit is dodging or not. Match on Unit Attacked and check whether the unit is also dodging.
     - In general, track when the event(s) last happened and check that data instead of getting it from the event log.
2. Access to OriginalState will be removed
   - Recommended migration:
     - Save the data somewhere and compare as needed. This gives more control over when data is saved.
