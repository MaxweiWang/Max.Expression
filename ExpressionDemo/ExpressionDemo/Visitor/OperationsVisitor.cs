using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionDemo.Visitor
{
    /// <summary>
    /// ExpressionVisitor:肯定得递归解析表达式目录树，因为不知道深度的一棵树
    /// 只有一个入口叫Visit  
    /// 首先检查是个什么类型的表达式，然后调用对应的protected virtual visit方法
    /// 得到结果继续去  检查类型--调用对应的visit方法--再检测--再调用。。。
    /// </summary>
    public class OperationsVisitor : ExpressionVisitor
    {
        public Expression Modify(Expression expression)
        {
            return base.Visit(expression);
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }

        /// <summary>
        /// 二元表达式解析
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.Add)
            {
                Expression left = base.Visit(b.Left);
                Expression right = base.Visit(b.Right);
                return Expression.Subtract(left, right);
            }

            return base.VisitBinary(b);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return base.VisitConstant(node);
        }
    }
}
