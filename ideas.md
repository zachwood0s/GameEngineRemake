# IDEAS

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




