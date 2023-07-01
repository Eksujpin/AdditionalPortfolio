// File Intro/SimpleExpr.java
// Java representation of expressions as in lecture 1
// sestoft@itu.dk * 2010-08-29

import java.util.Map;
import java.util.HashMap;

abstract class Expr { 
  abstract public int eval(Map<String,Integer> env);
  abstract public String fmt();
  abstract public String fmt2(Map<String,Integer> env);
  // ex 1.4.IV
  abstract public Expr Simplify();
}

class CstI extends Expr { 
  protected final int i;

  public CstI(int i) { 
    this.i = i; 
  }

  public int eval(Map<String,Integer> env) {
    return i;
  } 

  public String fmt() {
    return ""+i;
  }

  public String fmt2(Map<String,Integer> env) {
    return ""+i;
  }
  
  // ex 1.4.IV
  public Expr Simplify (){
    return this;
  } 

  public int getCstI(){
    return i;
  }

}

class Var extends Expr { 
  protected final String name;

  public Var(String name) { 
    this.name = name; 
  }

  public int eval(Map<String,Integer> env) {
    return env.get(name);
  } 

  public String fmt() {
    return name;
  } 

  public String fmt2(Map<String,Integer> env) {
    return ""+env.get(name);
  } 

  // ex 1.4.IV
  public Expr Simplify (){
    return this;
  } 

}

class Prim extends Expr { 
  protected final String oper;
  protected final Expr e1, e2;

  public Prim(String oper, Expr e1, Expr e2) { 
    this.oper = oper; this.e1 = e1; this.e2 = e2;
  }

  public int eval(Map<String,Integer> env) {
    if (oper.equals("+"))
      return e1.eval(env) + e2.eval(env);
    else if (oper.equals("*"))
      return e1.eval(env) * e2.eval(env);
    else if (oper.equals("-"))
      return e1.eval(env) - e2.eval(env);
    else
      throw new RuntimeException("unknown primitive");
  } 

  public String fmt() {
    return "(" + e1.fmt() + oper + e2.fmt() + ")";
  } 

  public String fmt2(Map<String,Integer> env) {
    return "(" + e1.fmt2(env) + oper + e2.fmt2(env) + ")";
  } 

  // ex 1.4.IV
  public Expr Simplify (){
    var se1 = e1.Simplify();
    var se2 = e2.Simplify();
    var ret1 = simpE(se1, se2);
    var ret2 = simpE(se2, se1);
    if (ret1 != null) return ret1;
    if (ret2 != null) return ret2;
    
    if (se2 instanceof CstI) {
        CstI c2 = (CstI) se2;
        if(oper.equals("-") && c2.getCstI() == 0) return se1;
    }
    if(oper.equals("-") && se1.fmt().equals(se2.fmt())) return new CstI(0);;
    return this;
  }

  private Expr simpE(Expr e1, Expr e2 ){
       if (e1 instanceof CstI) {
        CstI c1 = (CstI) e1;
        if(oper.equals("+") && c1.getCstI() == 0){
            return e2;  
        }
        if(oper.equals("*") && c1.getCstI() == 1){
            return e2;  
        }
        if(oper.equals("*") && c1.getCstI() == 0){
            return new CstI(0);    
        }
        }
     return null;
  }




}

public class SimpleExpr {
  public static void main(String[] args) {
    Expr e1 = new CstI(17);
    Expr e2 = new Prim("+", new CstI(3), new Var("a"));
    Expr e3 = new Prim("+", new Prim("*", new Var("b"), new CstI(9)),new Var("a"));
    
    Expr e4 = new Prim("+", new Var ("x"), new CstI (0));    
    Expr e5 = new Prim("+", new CstI (0), new CstI (1));
    Expr e6 = new Prim("+", new CstI (1), new CstI (0));
    Expr e7 = new Prim("*", new Prim("+", new CstI (0), new CstI (1)),new Prim("+", new Var ("x"), new CstI (0)));

    Map<String,Integer> env0 = new HashMap<String,Integer>();
    env0.put("a", 3);
    env0.put("c", 78);
    env0.put("baf", 666);
    env0.put("b", 111);

    System.out.println("Env: " + env0.toString());

    System.out.println(e1.fmt() + " = " + e1.fmt2(env0) + " = " + e1.eval(env0));
    System.out.println(e2.fmt() + " = " + e2.fmt2(env0) + " = " + e2.eval(env0));
    System.out.println(e3.fmt() + " = " + e3.fmt2(env0) + " = " + e3.eval(env0));

     // ex 1.4.IV
    System.out.println(e4.fmt() +  " = " + e4.Simplify().fmt());
    System.out.println(e5.fmt() +  " = " + e5.Simplify().fmt());
    System.out.println(e6.fmt() +  " = " + e6.Simplify().fmt());
    System.out.println(e7.fmt() +  " = " + e7.Simplify().fmt());

  }
}
