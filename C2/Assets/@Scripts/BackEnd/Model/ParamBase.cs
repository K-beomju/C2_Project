using BackEnd;

namespace C2Project.Model
{
    public abstract class ParamBase
    {
        public abstract Param GetParam();

        public Param GetParam(params string[] keys)
        {
            var allParam = GetParam();
            var ret = new Param();

            foreach (var key in keys)
            {
                if (!allParam.ContainsKey(key))
                    continue;

                ret.Add(key, allParam[key]);
            }

            return ret;
        }
    }
}