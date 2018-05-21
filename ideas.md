# IDEAS

## Entity Group Managers

    Right now the entities controll if they get matched with a group with their IsMatch function. I think it would be better
    if that logic was moved into something like a groupManager. This way they could be extended nicely without having to make 
    the entity extend them. The group managers could start with a base one that doesn't handle filters or anything and then
    extend that to deal with filters. Also I suggest that the subscribing and everything happens in the group manager just in
    case additional logic is required. I think this will clean up the entities nicely.

    Also the filter group would add the cached groups part
        Note: The cached groups are needed because if an entity is removed from a group on update when the filter fails,
              it will never be added back on update. To fix this I would suggest the 'filtered group manager' to have a list
              of viable groups, excluding the filter, that the entity could be added to.


## Fully modular

### How?

Lets say we have a method like pathfinding which we would like to open up to modding. 

    We Have an entity with components of person and location.
    We have a pathfinding system that acts on person, location, and path.

    When the path component gets added to our person. The path system would be triggered and add
    a route component to the person based off the specific algorithm.

    The question is: How do we open that pathfinding algorithm so that it can be overridden by a different
    one.

#### Some Type of Delegate override

    Lets say our path system has a pathfind method. All we would do is override that when initializing our scene.

    Pros:
        - Simple as can be

    Cons:
        - Not really easy to tell what is happening later on. No one really knows what pathfind is being used because it was overriden.
        - The pathfind method would have to be a global constant, or settable later which could be gross.

#### Letting the ecs do it's job

    Multiple options here... Most can work together quite nicely I think.

    One is allowing the premade game objects, prefabs, to be edited by a mod. Allow modders to remove and add components that should be attached to a prefab. This could be a little risky because they could completely break the game if done wrong. But they can do that already so ehh. 

    Another part would be allowing the modders to change what the systems look for. Allow them to change the systems matcher.

    Also allow systems to be added anywhere in the system pool, eg. before or after other systems.

    Pros:
        - The combination of all these would be incredibly flexible.
        - I think it would allow modders to edit any part of the game decently safely. or at least have the correct tools to.

    Cons:
        - Still a mod could completely destroy a whole gameplay aspect. 

    Rimworld example:
        In rimworld I wanted to create a food manager mod. I needed to add an extra property to all the colonists and then I'd also need to 
        rewrite the find food method to find only correct food. In the process I could destroy something very important.

        but here lets say we have a FoodManager system and it only deals with components of Position, and NeedsFood. It simply routes this position
        to the nearest food source (or has some additional logic, but essentially that's what it does). But the base game doesn't allow for
        restrictions on what food is selected. So we create a component called FoodRestriction and add it to the colonists' prefab.
        Now we could either add a system befor/after the FoodManager system to extend/override it's functionality, OR we could create a new system 
        FoodManagerWRestriction that takes Position, NeedsFood, and FoodRestriction and then change the existing FoodManager to accept Position, 
        NeedsFood, but not FoodRestriction. We have now routed all entity food needs to our FoodManagerWRestriction. This is also awesome because we        wouldn't even need to change the prefab, we could just add the FoodRestriction component at runtime to the colonist we wanted to restrict 
        and the Restriction system would automatically start handling it.


## Events

    I think events can be handled purely with reactive systems and components. Maybe a nice wrapper could be added around them to 
    hide the fact that components are just being used? Maybe that would be unnecessary and obscure what's actually happening though.
    I mean adding a component and having a reactive system react is exactly what we want to happen with an event so it's already done.
    
    Maybe create an "EventHandler" reactive system which takes a handler delegate, and an 'Event' to respond to. Deciding what that
    event is would be the hard part. 
        - Would it be a set of already created events like "Update", "ComponentAdded", etc. 
        - Or would you be allowed to add custom events. I think that would be done be adding and removing components. 
    Like add the EventComponent with
    an event name then the event handler system would respond and handle it with the correct handler. This might only work if my
    existing setup allows for more than one system of the same type. Otherwise one reactive system would have to know about all the
    event handlers. I think, if I can make it so that more than one system of the same type can be used then this would work great.
    Also, this would be even better if the groups had another function like "Filter" or something which did one additional check
    when adding an a component. This would allow me to have one event component and only have an event name. When the event component
    would be added to the entity all the EventSystems would check if it met the component list criteria and then the additional
    filter: In this case the event name. Making it so the reactive systems wouldn't need to check all the entities to make sure they
    were in the correct system. Also, this would be a lot better if the EventSystems allowed you to optionally pass in the 
    MatchFunction, so that we can guarantee that the Entity has all the required components for the event. If it wasn't passed in
    then we can just assume that an event component is attached but possibly nothing else (would be useful for just adding components
    to an entity when an event occurs)

    The boadcasting system in the method listed above would be as simple as adding an event with a given name. A wrapper could be 
    wrapped around that to make it a little nicer but really not needed. With this however, variables could not be passed, only the
    name of an event. Event parameters could be very useful and really necessary. Maybe this would be a good solution

        EventHandler<EventParams *optional*>{
            
            On update the event component's params are auto typecast to our generic type.
            If no eventparams type is provided then the only thing we can access is the name of the event.

        }

        Event<EventParams *optional*>{
            String: name
            EventParams: params
        }

    A base eventparams class would be needed and then it could be inherited later.

    The problem this also poses is how to broadcast all/group/individual. Do I add all the event components and then have some type
    of 'GarbageCleanup' to test if it wasn't registerd to any reactive system? Do I add it to specific groups right off the bat
    based off of an optional match function? 
    
        Both of those options would suggest that a wrapper would be a nice thing to have because it could handle the logic for that.
        Another option would be to include enough power into my systems to allow all of these things to happen, which I think is
        possible. I'll need to re-skim over all my code to make sure that can work without too much rework

    UPDATE: After looking, it looks as if I can add as many systems as I want of the same type. It'll be a little iffy when you try
    to call the GetSystem command because it will return only the first type so I'll need to add a GetSystems function to account for
    that but I think it'll still be just fine


