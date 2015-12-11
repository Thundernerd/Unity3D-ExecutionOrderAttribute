# Unity3D-ExecutionOrderAttribute
An attribute that sets a script's execution order

* Works on MonoBehaviours and ScriptableObjects
* No need to open the Script Execution Order settings menu

Code
------------------------------------------
![Testers](http://puu.sh/lRAca/dfebebda87.png)

All you need to do is add the *[ExecutionOrder(...)]* line on top of your class and you are good to go.

Result
------------------------------------------
![Result](http://puu.sh/lRA97/3ac2006656.png)

Every time your code gets recompiled the attributes get checked and updated in the Script Execution Order list.
