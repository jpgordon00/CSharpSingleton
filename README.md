# CSharpSingleton
A modern Singleton for C# with searching by arbitrary property, inheritance support, Instance properties, explicit instantiation and more.

## Why?
- Group classes by classname, full classname, or by an arbritrary proprerty 'Type'.
> Singleton's can be searched by their classname or full classname (namespace included) by specyfing the property 'MatchByClassName' or 'MatchByFullClassName'.

> Singleton's can be grouped by a 'Type' object if both 'MatchByClassName' and 'MatchByFullClassName' are false.
- Made for inheritance.
> By specyfing the property "BaseOnly", classes can be excluded in Singleton searches. 

> By specyting the property "BaseType", classes can be filtered using Find, FindAll, Instantiate and Destroy.

## Uses

## Limitations
* Classes with 'ImplicitInstantiation' set to false may be created multiple times without actually being persisted.
- Refer to the class properties '_conststructed' to see if a Singleton has been persisted, and/or overload the 'Constructor' and 'Destructor' functions.
