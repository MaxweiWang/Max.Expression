using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionDemo
{
    /// <summary>
    /// 展示表达式树，协助用的
    /// </summary>
    public class ExpressionTreeVisualizer
    {
        public static void Show()
        {
            //Expression<Func<int, int, int>> exp = (m, n) => m * n + 2;
            //Expression<Func<People, bool>> lambda = x => x.Age > 5&&x.Name.Length>2;
            //Expression<Func<People, bool>> lambda = x => x.Id.ToString().Equals("5");
            //Expression<Func<People, bool>> lambda = x => x.Id == 0 ? true : false;

            Expression<Func<People, PeopleCopy>> lambda = p =>
                        new PeopleCopy()
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Age = p.Age
                        };
        }
    }
}
