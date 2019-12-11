using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ExpressionDemo.Extend;
using System.Diagnostics;
using ExpressionDemo.MappingExtend;

namespace ExpressionDemo
{
    /// <summary>
    /// 认识/拼装 表达式目录树
    /// 拼装表达式
    /// 应用
    /// </summary>
    public class ExpressionTest
    {
        public static void Show()
        {
            {
                Func<int, int, int> func = (m, n) => m * n + 2;// new Func<int, int, int>((m, n) => m * n + 2);
                Expression<Func<int, int, int>> exp = (m, n) => m * n + 2;//lambda表达式声明表达式目录树
                                                                          //Expression<Func<int, int, int>> exp1 = (m, n) =>//只能一行 不能有大括号
                                                                          //    {
                                                                          //        return m * n + 2;
                                                                          //    };
                                                                          //Queryable    //a=>a.Id>3

                //表达式目录树：语法树，或者说是一种数据结构;可以被我们解析
                int iResult1 = func.Invoke(12, 23);
                int iResult2 = exp.Compile().Invoke(12, 23);//可以转换过去
            }
            {
                Expression<Func<int, int, int>> exp = (m, n) => m * n + 2;
                ParameterExpression parameterExpression = Expression.Parameter(typeof(int), "m");
                ParameterExpression parameterExpression2 = Expression.Parameter(typeof(int), "n");
                var multiply = Expression.Multiply(parameterExpression, parameterExpression2);
                var constant = Expression.Constant(2, typeof(int));
                var add = Expression.Add(multiply, constant);

                Expression<Func<int, int, int>> expression =
                    Expression.Lambda<Func<int, int, int>>(
                        add,
                        new ParameterExpression[]
                        {
                             parameterExpression,
                             parameterExpression2
                        });

                int iResult1 = exp.Compile().Invoke(11, 12);
                int iResult2 = expression.Compile().Invoke(11, 12);
            }

            //自己拼装表达式目录树
            {
                //常量
                ConstantExpression conLeft = Expression.Constant(345);
                ConstantExpression conRight = Expression.Constant(456);
                BinaryExpression binary = Expression.Add(conLeft, conRight);//345+456
                Expression<Action> actExpression = Expression.Lambda<Action>(binary, null);//()=>345+456
                //只能执行表示Lambda表达式的表达式目录树，即LambdaExpression或者Expression<TDelegate>类型。如果表达式目录树不是表示Lambda表达式，需要调用Lambda方法创建一个新的表达式
                actExpression.Compile()();//()=>345+456
            }

            {
                ParameterExpression paraLeft = Expression.Parameter(typeof(int), "a");//左边
                ParameterExpression paraRight = Expression.Parameter(typeof(int), "b");//右边
                BinaryExpression binaryLeft = Expression.Multiply(paraLeft, paraRight);//a*b
                ConstantExpression conRight = Expression.Constant(2, typeof(int));//右边常量
                BinaryExpression binaryBody = Expression.Add(binaryLeft, conRight);//a*b+2

                Expression<Func<int, int, int>> lambda =
                    Expression.Lambda<Func<int, int, int>>(binaryBody, paraLeft, paraRight);
                Func<int, int, int> func = lambda.Compile();//Expression Compile成委托
                int result = func(3, 4);
            }
            {
                Expression<Func<People, bool>> lambda = x => x.Id.ToString().Equals("5");
                ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");
                var field = Expression.Field(parameterExpression, typeof(People).GetField("Id"));
                var toString = typeof(People).GetMethod("ToString");
                var toStringCall = Expression.Call(field, toString, new Expression[0]);
                var equals = typeof(People).GetMethod("Equals");
                var constant = Expression.Constant("5", typeof(string));
                var equalsCall = Expression.Call(toStringCall, equals, new Expression[] { constant });
                Expression<Func<People, bool>> expression =
                    Expression.Lambda<Func<People, bool>>(equalsCall, new ParameterExpression[]
                        {
                        parameterExpression
                        });

                expression.Compile().Invoke(new People()
                {
                    Id = 11,
                    Name = "Eleven",
                    Age = 31
                });
            }

            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");
                Expression field = Expression.Field(parameterExpression, typeof(People).GetField("Id"));
                MethodCallExpression toString = Expression.Call(field, typeof(People).GetMethod("ToString"), new Expression[0]);
                ConstantExpression constantExpression = Expression.Constant("5", typeof(string));

                MethodCallExpression equals = Expression.Call(toString, typeof(People).GetMethod("Equals"), new Expression[] { constantExpression });
                Expression<Func<People, bool>> lambda = Expression.Lambda<Func<People, bool>>(equals, new ParameterExpression[]
                {
                    parameterExpression
                });
                bool bResult = lambda.Compile()(new People()
                {
                    Id = 11,
                    Name = "Eleven",
                    Age = 31
                });
            }
            {
                //以前根据用户输入拼装条件
                string sql = "SELECT * FROM USER WHERE 1=1";
                string name = "Eleven";//用户选择条件
                if (string.IsNullOrWhiteSpace(name))
                {
                    sql += $" and name like '%{name}%'";//应该参数化
                }


                //现在entityx framework查询的时候，需要一个表达式目录树
                IQueryable<int> list = null;
                //都不过滤 1
                if (true)//只过滤A 2
                {
                    //list=list.Where();
                    Expression<Func<People, bool>> exp1 = x => x.Id > 1;
                }
                if (true)//只过滤B 3
                {
                    //list=list.Where();
                    Expression<Func<People, bool>> exp2 = x => x.Age > 10;
                }

                //都过滤 4
                Expression<Func<People, bool>> exp3 = x => x.Id > 1 && x.Age > 10;
                //2个条件  4种可能  排列组合
                //3个条件  2的3次方

                //list.Where()

                //拼装表达式目录树，交给下端用
                //Expression<Func<People, bool>> lambda = x => x.Age > 5;
                ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");
                Expression propertyExpression = Expression.Property(parameterExpression, typeof(People).GetProperty("Age"));
                //Expression property = Expression.Field(parameterExpression, typeof(People).GetField("Id"));
                ConstantExpression constantExpression = Expression.Constant(5, typeof(int));
                BinaryExpression binary = Expression.GreaterThan(propertyExpression, constantExpression);//添加方法的
                Expression<Func<People, bool>> lambda = Expression.Lambda<Func<People, bool>>(binary, new ParameterExpression[]
                {
                    parameterExpression
                });
                bool bResult = lambda.Compile()(new People()
                {
                    Id = 11,
                    Name = "Eleven",
                    Age = 31
                });
            }

            {
                People people = new People()
                {
                    Id = 11,
                    Name = "Eleven",
                    Age = 31
                };
                PeopleCopy peopleCopy = new PeopleCopy()
                {
                    Id = people.Id,
                    Name = people.Name,
                    Age = people.Age
                };
                //硬编码 是不是写死了  只能为这两个类型服务   性能是最好的
               
                {
                    //反射 不同类型都能实现     
                    var result = ReflectionMapper.Trans<People, PeopleCopy>(people);
                }
                {
                    //序列化器  不同类型都能实现
                    var result = SerializeMapper.Trans<People, PeopleCopy>(people);
                }
                {
                    //1 通用   2 性能要高
                    //能不能动态的生成硬编码，缓存起来
                    var result = ExpressionMapper.Trans<People, PeopleCopy>(people);
                }
                {
                    var result = ExpressionMapper.Trans<People, PeopleCopy>(people);
                }
                {
                    var result = ExpressionGenericMapper<People, PeopleCopy>.Trans(people);
                }
                {
                    var result = ExpressionGenericMapper<People, PeopleCopy>.Trans(people);
                }
                //总结表达式目录树动态生成的用途了：
                //可以用来替代反射，因为反射可以通用，但是性能不够
                //生成硬编码，可以提升性能
                //automapper基于emit  动态生成硬编码

                //Expression<Func<People, PeopleCopy>> lambda = p =>
                //        new PeopleCopy()
                //        {
                //            Id = p.Id,
                //            Name = p.Name,
                //            Age = p.Age
                //        };
                //lambda.Compile()(people);

                ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "p");
                List<MemberBinding> memberBindingList = new List<MemberBinding>();
                foreach (var item in typeof(PeopleCopy).GetProperties())
                {
                    MemberExpression property = Expression.Property(parameterExpression, typeof(People).GetProperty(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }
                foreach (var item in typeof(PeopleCopy).GetFields())
                {
                    MemberExpression property = Expression.Field(parameterExpression, typeof(People).GetField(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }
                MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(PeopleCopy)), memberBindingList.ToArray());
                Expression<Func<People, PeopleCopy>> lambda = Expression.Lambda<Func<People, PeopleCopy>>(memberInitExpression, new ParameterExpression[]
                {
                    parameterExpression
                });
                Func<People, PeopleCopy> func = lambda.Compile();
                PeopleCopy copy = func(people);
            }
        }

        public static void MapperTest()
        {
            People people = new People()
            {
                Id = 11,
                Name = "Eleven",
                Age = 31
            };

            long common = 0;
            long generic = 0;
            long cache = 0;
            long reflection = 0;
            long serialize = 0;
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1000000; i++)
                {
                    PeopleCopy peopleCopy = new PeopleCopy()
                    {
                        Id = people.Id,
                        Name = people.Name,
                        Age = people.Age
                    };
                }
                watch.Stop();
                common = watch.ElapsedMilliseconds;
            }
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1000000; i++)
                {
                    PeopleCopy peopleCopy = ReflectionMapper.Trans<People, PeopleCopy>(people);
                }
                watch.Stop();
                reflection = watch.ElapsedMilliseconds;
            }
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1000000; i++)
                {
                    PeopleCopy peopleCopy = SerializeMapper.Trans<People, PeopleCopy>(people);
                }
                watch.Stop();
                serialize = watch.ElapsedMilliseconds;
            }
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1000000; i++)
                {
                    PeopleCopy peopleCopy = ExpressionMapper.Trans<People, PeopleCopy>(people);
                }
                watch.Stop();
                cache = watch.ElapsedMilliseconds;
            }
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1000000; i++)
                {
                    PeopleCopy peopleCopy = ExpressionGenericMapper<People, PeopleCopy>.Trans(people);
                }
                watch.Stop();
                generic = watch.ElapsedMilliseconds;
            }

            Console.WriteLine($"common = { common} ms");
            Console.WriteLine($"reflection = { reflection} ms");
            Console.WriteLine($"serialize = { serialize} ms");
            Console.WriteLine($"cache = { cache} ms");
            Console.WriteLine($"generic = { generic} ms");

        }


    }
}
