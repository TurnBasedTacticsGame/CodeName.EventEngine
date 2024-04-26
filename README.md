# CodeName.EventSystem

The event system works off of the following equations:
- State + Input = Next State
  - Applying input can give you the next state.
- State + Input = Event Log
  - Applying input can give you an event log.
- State + Event Log = Next State
    - Applying the event log can also give you the next state.
    - This allows for replays.

## GameStateTrackers

These keep track of game state and all events that are raised.

### GenerativeGameStateTracker

This tracker implements the equations:
- State + Input = Next State
- State + Input = Event Log

This is typically used on the server to generate an event log that can be sent to clients.

### RegenerativeGameStateTracker

This tracker implements the equation:
- State + Event Log = Next State

This is typically used on the client to play animations.

### ConstGameStateTracker

This tracker implements the equation:
- State + Const Event = State
  - This is not part of the original equations that the event system works off of.

This is typically used for querying the game's state. Game event handlers can match on the event and store results as part of the event.

#### Const Events

Const events are events that are used with the ConstGameStateTracker.
Contrary to their name, the events themselves CAN be modified, but the GameState CANNOT be modified.

## EventHandlers

These respond to events that are raised.

### Event Phases

Events can be in one of three phases:
1. Raised - Event has been raised, but can be prevented during this phase.
2. Confirmed - Event has been raised and can no longer be prevented.
3. Applied - Event has been applied (`event.Apply()` has been called).

### Event Matching

Event matching is the main way to detect when events happen and react to them.

```cs
// Matching for a event
// Note: This supports inheritance so you can match for a base class and be notified of all subclasses of that class
if (tracker.MatchOn<UnitDamagedEvent, GameState>(out var unitDamaged))
{
    Debug.Log($"{unitDamaged.UnitId} was damaged for {unitDamaged.Damage} damage")

    // Heal damaged unit for 1 HP
    await tracker.RaiseEvent(new UnitHealedEvent(unitDamaged.UnitId, 1));
}

// Matching for a event caused by another event
if (tracker.MatchOn<UnitDamagedEvent, GameState>(out var unitDamaged).CausedBy<UnitUsedAbilityEvent, GameState>(out _))
{
    Debug.Log($"{unitDamaged.UnitId} was damaged for {unitDamaged.Damage} damage by an ability")

    // Heal damaged unit for 1 HP after being damaged by an ability
    await tracker.RaiseEvent(new UnitHealedEvent(unitDamaged.UnitId, 1));
}
```

### IGameEventHandler

Uses Event Matching to _synchronously_ react to events. Can modify the game state directly or respond by raising additional events.

GameEventHandlers are the main way to modify the game state.

### IGameAnimationHandler (WIP)

Uses event matching to _asynchronously_ react to events. Cannot modify the game state or raise additional events.

GameAnimationHandlers are the main way to play animations.

Because animations might require storing state between different event phases, event IDs can be used to determine when an event is confirmed and applied. For example, a character can wind up for an attack during the Confirmed phase, then hit and wind back down during the Applied phase.

## StateTask

Custom task type that is designed to be ran synchronously. While this seems counter intuitive to the purpose of async-await, StateTask allows code to be suspended. This allows code to be written in an intuitive async-await style and be suspended while animations are playing.

StateTask works seamlessly with other task types, but using non-StateTasks in GameEventHandlers is not recommended. This is because most GameStateTrackers expect GameEventHandlers to run synchronously.

## FAQ

### When are events required?

- Non-deterministic code. This is supported, but must be done in a specific way. See [common desync issues](#common-desync-issues).
- When event handlers need to react to events that are happening.

### Accessing event results in event handlers

- Accessing GameState after applying GameEvent IS supported.

- Accessing GameEvent after applying the GameEvent is NOT supported.
  - This is enforced by GenerativeGameStateTracker and RegenerativeGameStateTracker.

```cs
// I recommend inlining this in the ApplyEvent call so it is not possible to access gameEvent
var gameEvent = new GameEvent();
await tracker.ApplyEvent(gameEvent);

// tracker.State will be updated
// gameEvent will be defensively copied and not contain any changes made to the event
```

### Common desync issues

- Duplicated entities -> Use entity IDs instead.
  - Serialized entities are duplicated and assigned a new ID
- Incorrect non-deterministic code (RNG) -> Save non-deterministic results to event log.

Duplicated entities:
```cs
// Incorrect - Unit will be duplicated in event log
await tracker.ApplyEvent(new DamageTakenEvent(unit));

// Correct - Save entity ID to event log
await tracker.ApplyEvent(new DamageTakenEvent(unit.Id));
```

Non-deterministic code:
```cs
// Incorrect - RNG not saved to event log
unit.Health -= Random.Range(0, 10);

// Correct - RNG saved to event log
await tracker.ApplyEvent(new DamageTakenEvent(unit.Id, Random.Range(0, 10)));
```

### Other desync issues

- Floating point inaccuracy
  - Can't fix, but should be very rare.
  - Use integers or fixed point math if this is an issue.

### Using the event matcher (need to verify syntax)

- The following are equivalent, but MatchOn allows for more advanced queries (if needed):
  - `if (tracker.MatchOn<NameOfEvent, GameState>(out var result)) {}`
  - `if (tracker.CurrentEvent is NameOfEvent) {}`
  - `if (eventNode.Event is NameOfEvent) {}` (This one may get removed)

## Planned changes

### Event Tree and List replacement

- Event Tree and List will be removed and replaced with an Event Stack
  - This should make logic more consistent when matching on events that are preceded by other events
    - Eg: Dodged an attack this turn vs Dodged an attack in the last X turns
    - With the current implementation, the Event List is used, but the Event List only tracks events in the current turn. This means checking for dodges in the last X turns has to be implemented in a different way.
  - Recommended migration:
    - Track whether a unit is dodging or not. Match on Unit Attacked and check whether the unit is also dodging.
    - In general, track when the event(s) last happened and check that data instead of getting it from the event log.

### IGameStateTracker.OriginalState removal

- Access to OriginalState will be removed
  - Recommended migration:
    - Save the data somewhere and compare as needed. This gives more control over when data is saved.
