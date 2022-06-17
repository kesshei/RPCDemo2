using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace RPCServerLib
{
    internal static class IlGeneratorExtension
    {
        public static void AddLdcI4(this ILGenerator ilGenerator, int index)
        {
            switch (index)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    break;
                default:
                    ilGenerator.Emit(OpCodes.Ldc_I4_S, index);
                    break;
            }
        }
		public static void EmitLoadArg(this ILGenerator ilGenerator, int index)
		{
			if (ilGenerator == null)
			{
				throw new ArgumentNullException("ilGenerator");
			}
			switch (index)
			{
				case 0:
					ilGenerator.Emit(OpCodes.Ldarg_0);
					return;
				case 1:
					ilGenerator.Emit(OpCodes.Ldarg_1);
					return;
				case 2:
					ilGenerator.Emit(OpCodes.Ldarg_2);
					return;
				case 3:
					ilGenerator.Emit(OpCodes.Ldarg_3);
					return;
				default:
					if (index <= 255)
					{
						ilGenerator.Emit(OpCodes.Ldarg_S, (byte)index);
						return;
					}
					ilGenerator.Emit(OpCodes.Ldarg, index);
					return;
			}
		}
	}
}
