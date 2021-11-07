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
  
## Examples
* Use a Singleton with an Instance property.
```c#
class MySIngleton : InstanceSingleton<MySingleton> {
}

// somewhere else
MySingleton instance = MySingleton.Instance;
```  
* Use a Singleton without implicit instantiation.
```c#
class MySIngleton : InstanceSingleton<MySingleton> {
  public override bool ImplicitInstantiation => false;
}

// somewhere else
MySingleton instance = MySingleton.Instance; // is null
Singleton.Instantiate<MySingleton>();
// NOTE: there are many options for 'Instantiate'
// include your own type and base type, using null to avoid matching either
// or include a generic type in 'Instantiate'
// or invoke 'InstantiateBase' with a generic to ignore the type of the given generic.

```  
* Find a Singleton by specyfing the 'Type' property and by ClassName.
```c#
class Mode : Singleton {
  public override bool BaseOnly => true;
  public override bool MatchByClassName => true;
...
}
class PreMode : Mode {
  public override bool BaseOnly => false;
  ...
}
class PostMode : Mode {
  public override bool BaseOnly => true;
  
  // do not match by class name, instead specify a property to match
  public override bool MatchByClassName => false;
  public override object Type => "PostMode";
}

class Server : {  
  static Server() {
    List<Mode> Modes = Singleton.FindAll<Mode>(); // list of all subclasses of mode
    
    // find mode by classname
    // note you don't need to cast polymorphic return types
  
    // find only subtypes of 'Mode' that match the given 'Type'
    PreMode mode = Singleton.Find<Mode>("PreMode");
    PostMode mode = Singleton.Find<Mode>("PostMode");
  
    // exclude the generic to search for any matching Type 
    mode = Singleton.Find("PostMode");
  }
}
```  
