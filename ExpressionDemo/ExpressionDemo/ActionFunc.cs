using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionDemo
{
    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, in T17>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17);


    /// <summary>
    /// .Net Framework3.0  Linq/task
    /// </summary>
    public class ActionFunc
    {
        public void Show()
        {
            //Action 0到16个参数的 没有返回值  泛型委托
            Action action1 = () => { };
            Action<int> action2 = i => Console.WriteLine(i);
            Action<int, string, DateTime, ActionFunc, int, string, DateTime, ActionFunc, int, string, DateTime, ActionFunc, int, string, DateTime, ActionFunc> action17 = null;

            //Func   0到16个参数的 带返回值  泛型委托
            Func<int> func1 = () => 1;
            Func<int, string> func2 = i => i.ToString();
            Func<int, string, DateTime, ActionFunc, int, string, DateTime, ActionFunc, int, string, DateTime, ActionFunc, int, string, DateTime, ActionFunc, string> func17 = null;

            this.DoNothing(action1);

            //NoReturnNoPara method = () => { };
            //this.DoNothing(method);
            //因为NoReturnNoPara和Action不是同一个类型
            //Student Teacher   大家属性都差不多，但是实例之间是不能替换的，因为我们没啥关系
            //很多委托长得一样，参数列表 返回值类型都一样，但是不能通用
            //在不同的框架组件定义各种各样的相同委托，就是浪费   比如ThreadStart委托
            //所以为了统一，就全部使用标准的Action  Func
        }

        public delegate void NoReturnNoPara();

        private void DoNothing(Action act)
        {
            act.Invoke();
        }


    }
}
