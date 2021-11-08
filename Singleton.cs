using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class SingletonUtils {

  public static bool TypeEquals(System.Type type, System.Type typee) {
    return !type.IsGenericType ? type == typee : (type.GetGenericTypeDefinition() == typee);
  }

  public static bool IsSameOrInherits(Type potentialBase, Type potentialDescendant) {
    return potentialDescendant.IsSubclassOf(potentialBase) ||
      potentialDescendant == potentialBase;
  }

  public static System.Type RecursiveTypeSearch(System.Type type, params System.Type[] types) {
     if (type == null) return type;
    if (types.ToList().Find(t => TypeEquals(t, type)) != null) return type;
    return RecursiveTypeSearch(type.BaseType, types);
  }

  public static bool InheritsOrImplements(this Type child, Type parent) {
    parent = ResolveGenericTypeDefinition(parent);

    var currentChild = child.IsGenericType ?
      child.GetGenericTypeDefinition() :
      child;

    while (currentChild != typeof (object)) {
      if (parent == currentChild || HasAnyInterfaces(parent, currentChild))
        return true;

      currentChild = currentChild.BaseType != null &&
        currentChild.BaseType.IsGenericType ?
        currentChild.BaseType.GetGenericTypeDefinition() :
        currentChild.BaseType;

      if (currentChild == null)
        return false;
    }
    return false;
  }

  private static bool HasAnyInterfaces(Type parent, Type child) {
    return child.GetInterfaces()
      .Any(childInterface => {
        var currentInterface = childInterface.IsGenericType ?
          childInterface.GetGenericTypeDefinition() :
          childInterface;

        return currentInterface == parent;
      });
  }

  private static Type ResolveGenericTypeDefinition(Type parent) {
    var shouldUseGenericType = true;
    if (parent.IsGenericType && parent.GetGenericTypeDefinition() != parent)
      shouldUseGenericType = false;

    if (parent.IsGenericType && shouldUseGenericType)
      parent = parent.GetGenericTypeDefinition();
    return parent;
  }
}

public class InstanceSingleton < T >: Singleton where T: Singleton, new() {
  private static T _Instance;

  public static T Instance {
    get {
      if (_Instance == null) _Instance = Find < T > ();
      return _Instance;
    }
  }
}

// subclass this class for a singleton like structure
// NOTE: 'ImplicitInstantiation' classes are still created but not persisted

public class Singleton {

  public Singleton() {}

  // find the list of all subclasses once
  private static IEnumerable < Singleton > types = new List < Singleton > ();

  // shortcut to all 'Types'
  public static List < object > AllTypes = new List < object > ();

  // shortcut to all 'BaseTypes'
  public static List < object > AllBaseTypes = new List < object > ();

  static Singleton() {
    types = typeof (Singleton)
      .Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof (Singleton)) && !t.IsAbstract).Select(t => {
        if (t == typeof (Singleton)) return null; // ignore base types
        if (SingletonUtils.TypeEquals(t, typeof (InstanceSingleton < > ))) return null; // ignore base types
        Singleton utr;
        System.Type baseType = SingletonUtils.RecursiveTypeSearch(t, typeof (Singleton), typeof (InstanceSingleton < > ));
        if (SingletonUtils.TypeEquals(baseType, typeof (InstanceSingleton < > ))) {
          var typ = t.BaseType.MakeGenericType(t);
          utr = (Singleton) Activator.CreateInstance(typ);
          if (!utr.ImplicitInstantiation) return null;
          return utr;
        }
        utr = (Singleton) Activator.CreateInstance(t);
        if (!utr.ImplicitInstantiation) return null;
        AllTypes.Add(utr.RealType);
        AllBaseTypes.Add(utr.BaseType);
        utr.Constructor();
        utr._constructed = true;
        return utr;
      });
    types = types.Where(t => t != null);
  }

  // optionally instatiate types by 'BaseType'
  // NOTE: not by types
  public static void Instantiate(object type = null, object baseType = null) {
    types = types.Concat < Singleton > (typeof (Singleton)
      .Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof (Singleton)) && !t.IsAbstract).Select(t => {
        if (t == typeof (Singleton)) return null; // ignore base types
        if (SingletonUtils.TypeEquals(t, typeof (InstanceSingleton < > ))) return null; // ignore base types
        Singleton utr = null;
        System.Type bt = SingletonUtils.RecursiveTypeSearch(t, typeof (Singleton), typeof (InstanceSingleton < > ));
        if (SingletonUtils.TypeEquals(bt, typeof (InstanceSingleton < > ))) {
          var typ = t.BaseType.MakeGenericType(t);
          utr = (Singleton) Activator.CreateInstance(typ);
        } else {
          utr = (Singleton) Activator.CreateInstance(t);
        }
        if (utr.ImplicitInstantiation) return null; // ignore UTR=ImplicitInstantitian
        if (AllTypes.Contains(utr.RealType)) return null; // ignore existing types already
        Console.WriteLine(t.FullName);
        if (baseType != null && utr.BaseType != baseType) return null; // if baseType is null then ignore if mismatching
        if (type != null && utr.RealType != type) return null; // same as bt
        utr.Constructor();
        utr._constructed = true;
        AllTypes.Add(utr.RealType);
        AllBaseTypes.Add(utr.BaseType);
        return utr;
      }));
    types = types.Where(t => t != null);
  }

  public static void InstantiateBase < T > () where T: Singleton, new() {
    Singleton tutr = new T() as Singleton;
    Instantiate(null, tutr.BaseType);
  }

  public static void Instantiate < T > () where T: Singleton, new() {
    Singleton tutr = new T() as Singleton;
    Instantiate(tutr.RealType, tutr.BaseType);
  }

  // optionally delete types by 'Type or 'BaseType'
  // NOTE: ignore params to not filter by them
  public static void Destroy(object type = null, object baseType = null) {
    // keep UTR's where 'ImplicitInstantiation' is true
    // if baseType isnt null then remove non matching BaseType's 
    types = types.Where(utr => {
      bool r = utr.ImplicitInstantiation && (utr.BaseType != baseType || baseType == null);
      if (type != null && utr.RealType == type) r = false;
      if (!r) {
        AllTypes.Remove(utr.RealType);
        AllBaseTypes.Remove(utr.BaseType);
        utr.Destructor();
        utr._constructed = false;
      }
      return r && utr != null;
    });
  }

  public static void Destroy < T > () where T: Singleton, new() {
    Singleton tutr = new T() as Singleton;
    Destroy(tutr.Type, tutr.BaseType);
  }
  // returns an subclass of T
  // generic type used for base type
  // searches by param class type or Type if defined
  public static T Find < T > (object type = null, object baseType = null) where T: Singleton, new() {
    Singleton found = null;
    Singleton temp = (new T()) as Singleton;
    baseType = baseType == null ? temp.BaseType : baseType;
    type = type == null ? temp.RealType : type;
    int i = 0;
    foreach(var v in types) {
      if (!v.BaseOnly) {
        if (baseType == null || v.BaseType == baseType) { // match baseTypes if baseType != not null
          if (v.RealType == type) {
            found = v;
            break;
          } // match Object.GetType if Singleton.Type is null
        }
      }
      i += 1;
    }
    return found == null ?
      default (T) : (T)(object) found;
  }

    public static T Find(object type = null, object baseType = null) where T : Singleton, new()
    {
        return Find<Singleton>(type, baseType);
    }

    // returns all matching BaseTypes given a base type param
    public static List < T > FindAll < T > (object baseType = null) where T: Singleton, new() {
    List < T > list = new List < T > ();
    // case: find in types and add to cache
    Singleton temp = (new T() as Singleton);
    foreach(var v in types) {
      if (!v.BaseOnly) {
        if (baseType == null || v.BaseType == baseType) // match baseTypes
        {
          list.Add((T)(object) v);
        }
      }
    }
    return list;
  }


    // optionally implement to be notified when a UTR with 'ImplicitInstantiation'=false is created or deleted
    // can be used if 'ImplicitInstantion'= true
    public virtual void Constructor() {}

  public virtual void Destructor() {}

  /* 
  	UTR's can check if they have been 'really' constructed, since 'ImplicitConstruction' constructs multiple times
  */
  internal bool _constructed = false;
  public bool Constructed {
    get => _constructed;
  }

  /*
			True to ignore in 'Find/FindAll' and in AllTypes
			By default is only true when Type is nill
		*/
  public virtual bool BaseOnly {
    get => false;
  }

  /*
			Create this class by default when the runtime is initiated.
			If fault, you must invoke 'InstantiateImplicits' and 'DestroyImplicits'
		*/
  public virtual bool ImplicitInstantiation {
    get => true;
  }

  /*
			True to match by classname instead of Type.
			Enforces 'MatchByFullClassName' to false.
		*/
  internal bool _MatchByClassName = true;
  public virtual bool MatchByClassName {
    get => _MatchByClassName;
    set {
      _MatchByClassName = value;
      if (value) MatchByFullClassName = false;
    }
  }

  /*
			True to match by namespace and classname instead of Type.
			Enforces 'MatchBylassName' to false.
		*/
  internal bool _MatchByFullClassName = false;
  public virtual bool MatchByFullClassName {
    get => _MatchByFullClassName;
    set {
      _MatchByFullClassName = value;
      if (value) MatchByClassName = false;
    }
  }

  // classes matched by this object or classname
  // virtual to be null by default ( gets ignored in search and types )
  public virtual object Type {
    get {
      return null;
    }
  }

  // either gets this class's name, fullname, or type
  public object RealType => MatchByClassName ? GetType().Name : (MatchByFullClassName ? GetType().FullName : Type);

    // optional base type to group classes of this type
    public virtual object BaseType => "base";
}
