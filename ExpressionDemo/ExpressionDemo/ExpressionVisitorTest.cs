using ExpressionDemo.Extend;
using ExpressionDemo.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionDemo
{
    /// <summary>
    /// 表达式访问者
    /// </summary>
    public class ExpressionVisitorTest
    {
        private static int Get(int k)
        {
            return 5;
        }

        public static void Show()
        {
            {
                //修改表达式目录树
                Expression<Func<int, int, int>> exp = (m, n) => m * n + 2 + 3 + Get(m);
                //我想修改下，变成m*n-2
                OperationsVisitor visitor = new OperationsVisitor();
                //visitor.Visit(exp);
                Expression expNew = visitor.Modify(exp);
            }
            {
                var source = new List<People>().AsQueryable();//DbSet
                var result = source.Where<People>(p => p.Age > 5)
                    .Where(p => p.Name.Equals("123"))
                    .Where(p => p.Age < 10)
                    //.ToList()
                    ;//SELECT * FROM People Where Age> '5'

                Expression<Func<People, bool>> lambda = x => x.Age > 5;

                ConditionBuilderVisitor vistor = new ConditionBuilderVisitor();
                vistor.Visit(lambda);
                Console.WriteLine(vistor.Condition());
            }


            {
                //ORM 把数据库映射到程序内存  通过操作对象来完成对数据库的管理
                //屏蔽数据库，开发者完全不需要知道

                //Linq To Object   实现IEnumerable
                //Enumerable
                //Queryable
                //IQueryable

                Expression<Func<People, bool>> lambda = x => x.Age > 5 && x.Id > 5
                                                         && x.Name.StartsWith("1")
                                                         && x.Name.EndsWith("1")
                                                         && x.Name.Contains("1");

                string sql = string.Format("Delete From [{0}] WHERE {1}"
                    , typeof(People).Name
                    , " [Age]>5 AND [ID] >5"
                    );
                ConditionBuilderVisitor vistor = new ConditionBuilderVisitor();
                vistor.Visit(lambda);
                Console.WriteLine(vistor.Condition());
            }
            {
                Expression<Func<People, bool>> lambda = x => x.Age > 5 && x.Name == "A" || x.Id > 5;
                ConditionBuilderVisitor vistor = new ConditionBuilderVisitor();
                vistor.Visit(lambda);
                Console.WriteLine(vistor.Condition());
            }
            {
                Expression<Func<People, bool>> lambda = x => x.Age > 5 || (x.Name == "A" && x.Id > 5);
                ConditionBuilderVisitor vistor = new ConditionBuilderVisitor();
                vistor.Visit(lambda);
                Console.WriteLine(vistor.Condition());
            }
            {
                Expression<Func<People, bool>> lambda = x => (x.Age > 5 || x.Name == "A") && x.Id > 5;
                ConditionBuilderVisitor vistor = new ConditionBuilderVisitor();
                vistor.Visit(lambda);
                Console.WriteLine(vistor.Condition());
            }

            #region 表达式链接
            {
                Expression<Func<People, bool>> lambda1 = x => x.Age > 5;
                Expression<Func<People, bool>> lambda2 = x => x.Id > 5;
                Expression<Func<People, bool>> lambda3 = lambda1.And(lambda2);
                Expression<Func<People, bool>> lambda4 = lambda1.Or(lambda2);
                Expression<Func<People, bool>> lambda5 = lambda1.Not();
                Do1(lambda3);
                Do1(lambda4);
                Do1(lambda5);
            }
            #endregion


        }
        #region PrivateMethod
        private static void Do1(Func<People, bool> func)
        {
            List<People> people = new List<People>();
            people.Where(func);
        }
        private static void Do1(Expression<Func<People, bool>> func)
        {
            List<People> people = new List<People>()
            {
                new People(){Id=4,Name="123",Age=4},
                new People(){Id=5,Name="234",Age=5},
                new People(){Id=6,Name="345",Age=6},
            };

            List<People> peopleList = people.Where(func.Compile()).ToList();
        }

        private static IQueryable<People> GetQueryable(Expression<Func<People, bool>> func)
        {
            List<People> people = new List<People>()
            {
                new People(){Id=4,Name="123",Age=4},
                new People(){Id=5,Name="234",Age=5},
                new People(){Id=6,Name="345",Age=6},
            };

            return people.AsQueryable<People>().Where(func);
        }
        #endregion
    }
}
