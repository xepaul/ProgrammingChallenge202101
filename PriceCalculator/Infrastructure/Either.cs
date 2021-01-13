using System;

namespace PriceCalculator.Infrastructure
{
    public struct Either<l, r>
    {

        private readonly r _right;
        private readonly l _left;
        private readonly bool _isLeft;  // we need both _isLeft and _isRight here, 
        private readonly bool _isRight; //as the either may be called with the empty constructor which can not be restricted

        public Either(r right)
        {
            _right = right;
            _isRight = true;
            _left = default;
            _isLeft = false;
        }

        public Either(l left)
        {
            _right = default;
            _isRight = false;
            _left = left;
            _isLeft = true;
        }

        public b either<b>(Func<l, b> leftFunc, Func<r, b> rightFunc)
        {
            if (_isLeft && _isRight) throw new Exception($"Logically impossible type:{typeof(r)}");
            if (!_isLeft && !_isRight || _isLeft && _isRight) throw new Exception($"NotInitialized type:{typeof(r)}");
            return _isLeft ? leftFunc(_left)
                : rightFunc(_right);
        }
        public static Either<l, r> Left(l left) => new (left);
        public static Either<l, r> Right(r right) => new (right);
    }
    public static class Either<t>
    {
        public static Either<t, r> Pure<r>(r v) => Right(v);
        public static Either<t, r> Return<r>(r v) => Right(v);
        public static Either<t, r> Right<r>(r right) => Either<t, r>.Right(right);
        public static Either<l, t> Left<l>(l left) => Either<l, t>.Left(left);

    }
}