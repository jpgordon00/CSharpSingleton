# CSharpSingleton
A modern Singleton for C# with searching by arbitrary property, inheritance support, Instance properties, explicit instantiation and more.

## Why?
- Constrol exactly when Singleton's are instantiated.
> Singleton's are created whenever the first Singleton is accesed.

> Specify the property 'ImplicitInstantiation' to be false to avoid creating Singleton's at runtime. See 'Limitations' for clarification on why Singleton object's may be created but not persisted when this property is false.

> By using the functions 'Singleton.Instantiate' and 'Singleton.Destroy', you can destroy or instantiate any specified Singleton's. Invoke 'Singleton.Instantiate()' to instantiate all Singleton's who are not persisted. Invoke 'Singleton.Instantiate<SingletonType>' to instantiate an exactly matching Singleton. Invoke 'Singleton.InstantiateBase<SingletonType>' to instantiate just the base types associated with the generic paramter, which is very useful for inheritance. For example, instantiate/destroy all SIngleton's of a certain parent.  

- Group classes by classname, full classname, or by an arbritrary proprerty 'Type'.
> Singleton's can be searched by their classname or full classname (namespace included) by specyfing the property 'MatchByClassName' or 'MatchByFullClassName'.

> Singleton's can be grouped by a 'Type' object if both 'MatchByClassName' and 'MatchByFullClassName' are false.
- Made for inheritance.
> By specyfing the property "BaseOnly", classes can be excluded in Singleton searches. 

> By specyting the property "BaseType", classes can be filtered using Find, FindAll, Instantiate and Destroy.

## Use Cases
- For any Singleton use case.
- For any Singleton that you want to find via a string or alternative property.
- For any Singleton where you want explicit instantiation and destruction.
> Set 'ImplciitInstantiation' to false and override 'Construct' and "Destroy'.


## Limitations
* Classes with 'ImplicitInstantiation' set to false may be created multiple times without actually being persisted.
- Refer to the class properties '_conststructed' to see if a Singleton has been persisted, and/or overload the 'Constructor' and 'Destructor' functions.
