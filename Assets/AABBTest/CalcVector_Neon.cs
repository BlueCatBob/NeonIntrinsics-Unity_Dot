
using Unity.Burst;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm.Neon;

public struct NeonVector3
{
    public float[] v3x1;
    public float[] v3y1;
    public float[] v3z1;
    public float[] v3x2;
    public float[] v3y2;
    public float[] v3z2;

    public int Length { get; private set; }
    public NeonVector3(int length)
    {
        Length = length;
        v3x1 = new float[length];
        v3y1 = new float[length];
        v3z1 = new float[length];
        v3x2 = new float[length];
        v3y2 = new float[length];
        v3z2 = new float[length];
    }
}

public unsafe struct NeonVector3Result
{
    public float* v3x;
    public float* v3y;
    public float* v3z;

    public int Length { get; private set; }
    public NeonVector3Result(float* v3x, float* v3y, float* v3z, int length)
    {
        Length = length;
        this.v3x = v3x;
        this.v3y = v3y;
        this.v3z = v3z;
    }
}


[BurstCompile]
public static class CalcVector_Neon
{
    [BurstCompile]
    public static unsafe void DotProductNeon(float* fArr1, float* fArr2, float* result, int length, int randomCount)
    {
        if (IsNeonSupported)
        {
            float* fptr1 = fArr1;
            float* fptr2 = fArr2;

            const int elementsPerIteration = 1;
            int iterations = length / elementsPerIteration;

            for (int j = 0; j < randomCount; j++)
            {
                for (int i = 0; i < iterations; i++)
                {
                    //var v1 = vld1_f32(fptr1);
                    //var v2 = vld1_f32(fptr2);
                    //var temp = vmulx_f32(v1, v2);
                    //result[i] = vaddv_f32(temp);

                    //fptr1 += elementsPerIteration;
                    //fptr2 += elementsPerIteration;
                    result[i] = vmulxs_f32(fptr1[i], fptr2[i]);
                }
            }

        }
    }

    public static unsafe void DotProductWitoutBurst(float* fArrX1, float* fArrX2, float* fArrY1, float* fArrY2, float* fArrZ1, float* fArrZ2, float* resultX, float* resultY, float* resultZ, int length, int randomCount)
    {
        const int elementsPerIteration = 1;
        int iterations = length / elementsPerIteration;
        for (int j = 0; j < randomCount; j++)
        {
            for (int i = 0; i < iterations; i++)
            {
                resultX[i] = fArrX1[i] * fArrX2[i];
                resultY[i] = fArrY1[i] * fArrY2[i];
                resultZ[i] = fArrZ1[i] * fArrZ2[i];
            }
        }
    }


    [BurstCompile]
    public static unsafe void DotProductBurst(float* fArrX1, float* fArrX2, float* fArrY1, float* fArrY2, float* fArrZ1, float* fArrZ2, float* resultX, float* resultY, float* resultZ, int length, int randomCount)
    {
        const int elementsPerIteration = 1;
        int iterations = length / elementsPerIteration;
        for (int j = 0; j < randomCount; j++)
        {
            for (int i = 0; i < iterations; i++)
            {
                resultX[i] = fArrX1[i] * fArrX2[i];
                resultY[i] = fArrY1[i] * fArrY2[i];
                resultZ[i] = fArrZ1[i] * fArrZ2[i];
            }
        }
    }

    [BurstCompile]
    public static unsafe void DotProductNeon(float* fArrX1, float* fArrX2, float* fArrY1, float* fArrY2, float* fArrZ1, float* fArrZ2, float* resultX, float* resultY, float* resultZ, int length, int randomCount)
    {
        const int elementsPerIteration = 128 / 32;
        int iterations = length / elementsPerIteration;
        if (IsNeonSupported)
        {
            var tempV128 = new v128();
            for (int j = 0; j < randomCount; j++)
            {
                float* fptrX1 = fArrX1;
                float* fptrX2 = fArrX2;

                float* fptrY1 = fArrY1;
                float* fptrY2 = fArrY2;
                float* fptrZ1 = fArrZ1;
                float* fptrZ2 = fArrZ2;
                for (int i = 0; i < iterations; i++)
                {
                    var v11 = vld1q_f32(fptrX1);
                    var v12 = vld1q_f32(fptrX2);
                    var v21 = vld1q_f32(fptrY1);
                    var v22 = vld1q_f32(fptrY2);
                    var v31 = vld1q_f32(fptrZ1);
                    var v32 = vld1q_f32(fptrZ2);
                    tempV128 = vmulq_f32(v11, v12);
                    resultX[i * elementsPerIteration] = tempV128.Float0;
                    resultX[i * elementsPerIteration + 1] = tempV128.Float1;
                    resultX[i * elementsPerIteration + 2] = tempV128.Float2;
                    resultX[i * elementsPerIteration + 3] = tempV128.Float3;
                    tempV128 = vmulq_f32(v21, v22);
                    resultY[i * elementsPerIteration] = tempV128.Float0;
                    resultY[i * elementsPerIteration + 1] = tempV128.Float1;
                    resultY[i * elementsPerIteration + 2] = tempV128.Float2;
                    resultY[i * elementsPerIteration + 3] = tempV128.Float3;
                    tempV128 = vmulq_f32(v31, v32);
                    resultZ[i * elementsPerIteration] = tempV128.Float0;
                    resultZ[i * elementsPerIteration + 1] = tempV128.Float1;
                    resultZ[i * elementsPerIteration + 2] = tempV128.Float2;
                    resultZ[i * elementsPerIteration + 3] = tempV128.Float3;
                    fptrX1 += elementsPerIteration;
                    fptrX2 += elementsPerIteration;
                    fptrY1 += elementsPerIteration;
                    fptrY2 += elementsPerIteration;
                    fptrZ1 += elementsPerIteration;
                    fptrZ2 += elementsPerIteration;
                }
            }

        }
    }

    [BurstCompile]
    public static unsafe void SumNeon(float* fArr1, float* fArr2, float* fArr3, float* result, int length)
    {
        const int elementsPerIteration = 1;
        int iterations = length / elementsPerIteration;
        if (IsNeonSupported)
        {
            float* fptr1 = fArr1;
            float* fptr2 = fArr2;
            float* fptr3 = fArr3;



            for (int i = 0; i < iterations; i++)
            {
                var v1 = vld1_f32(fptr1);
                var v2 = vld1_f32(fptr2);
                var v3 = vld1_f32(fptr3);
                var sum23 = vadd_f32(v2, v3);
                var sum123 = vadd_f32(v1, sum23);
                result[i] = sum123.Float0;
                fptr1 += elementsPerIteration;
                fptr2 += elementsPerIteration;
                fptr3 += elementsPerIteration;
            }
        }
        else
        {
            for (int i = 0; i < iterations; i++)
            {
                result[i] = fArr1[i] + fArr2[i] + fArr3[i];
            }
        }
    }



    static NeonVector3 GenerateNeonV3(Vector3[] v3array1, Vector3[] v3array2, int length)
    {
        NeonVector3 t;
        if (length != v3array1.Length || length != v3array2.Length || length <= 0)
        {
            return new NeonVector3(0);
        }
        t = new NeonVector3(length);
        for (int i = 0; i < length; i++)
        {
            t.v3x1[i] = v3array1[i].x;
            t.v3y1[i] = v3array1[i].y;
            t.v3z1[i] = v3array1[i].z;

            t.v3x2[i] = v3array2[i].x;
            t.v3y2[i] = v3array2[i].y;
            t.v3z2[i] = v3array2[i].z;
        }
        return t;
    }

    public static NeonVector3 GenerateNeonV3(Vector3 v3_1, Vector3 v3_2)
    {
        NeonVector3 t = new NeonVector3(1);
        t.v3x1[0] = v3_1.x;
        t.v3y1[0] = v3_1.y;
        t.v3z1[0] = v3_1.z;

        t.v3x2[0] = v3_2.x;
        t.v3y2[0] = v3_2.y;
        t.v3z2[0] = v3_2.z;
        return t;
    }

    static unsafe Vector3[] TransformToV3(NeonVector3Result neonV3Result)
    {
        Vector3[] tArray = new Vector3[neonV3Result.Length];
        Vector3 t = Vector3.zero;
        for (int i = 0; i < neonV3Result.Length; i++)
        {
            t.x = *(neonV3Result.v3x);
            t.y = *(neonV3Result.v3y);
            t.z = *(neonV3Result.v3z);

            tArray[i] = t;

            neonV3Result.v3x++;
            neonV3Result.v3y++;
            neonV3Result.v3z++;
        }
        return tArray;
    }
}
