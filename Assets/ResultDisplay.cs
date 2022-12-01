using System.Diagnostics;
using System.Text;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;


public enum FunctionType
{
    WithoutBurst,
    Burst,
    Neon
}
public class ResultDisplay : MonoBehaviour
{
    public Text m_Text;


    public Text test;

    int seed = 123456789;
    static float maxInclude = 100.0f;
    static float minInclude = -100.0f;
    static int targetCount = 1000000;


    static float _randomSum = 0;
    static float _randomDotSum = 0;

    bool _isInit;

    Stopwatch _sw;
    private void Awake()
    {

        Random.InitState(seed);
        _isInit = false;
        _sw = new Stopwatch();
    }

    void Start()
    {
        // Ramp length and number of trials
        //const int rampLength = 1027;
        //const int trials = 1000000;

        //m_Text.text = DoCalc(rampLength, trials);




        test.text = DotTest();
        _isInit = true;
    }


    public unsafe float[] Test(float[] target, FunctionType ft, int randomCount, int length, out float sum)
    {
        sum = FloatArraySum(target);
        float[] reXa = target;
        float[] reXb = target;
        float[] reYa = target;
        float[] reYb = target;
        float[] reZa = target;
        float[] reZb = target;
        float[] resultXSum = new float[length];
        float[] resultYSum = new float[length];
        float[] resultZSum = new float[length];
        float[] result = new float[length];
        fixed (float* fxa = reXa, fxb = reXb, fya = reYa, fyb = reYb, fza = reZa, fzb = reZb, xSum = resultXSum, ySum = resultYSum, zSum = resultZSum, re = result)
        {
            //CalcVector_Neon.DotProductNeon(fxa, fxb, xSum, length);
            //CalcVector_Neon.DotProductNeon(fya, fyb, ySum, length);
            //CalcVector_Neon.DotProductNeon(fza, fzb, zSum, length);
            switch (ft)
            {
                case FunctionType.WithoutBurst:
                    CalcVector_Neon.DotProductWitoutBurst(fxa, fxb, fya, fyb, fza, fzb, xSum, ySum, zSum, length, randomCount);

                    break;
                case FunctionType.Burst:
                    CalcVector_Neon.DotProductBurst(fxa, fxb, fya, fyb, fza, fzb, xSum, ySum, zSum, length, randomCount);
                    break;
                case FunctionType.Neon:
                    CalcVector_Neon.DotProductNeon(fxa, fxb, fya, fyb, fza, fzb, xSum, ySum, zSum, length, randomCount);
                    break;
                default:
                    break;
            }
            CalcVector_Neon.SumNeon(xSum, ySum, zSum, re, length);

        }
        return result;
    }


    private unsafe string DoCalc(short rampLength, int trials)
    {
        var sb = new StringBuilder();

        // Generate two input vectors
        // (0, 1, ..., rampLength - 1)
        // (100, 101, ..., 100 + rampLength-1)
        var ramp1 = generateRamp(0, rampLength);
        var ramp2 = generateRamp(100, rampLength);
        var noBurstPerfMarker = new ProfilerMarker("Not Bursted");
        var burstPerfMarker = new ProfilerMarker("Bursted");

        fixed (short* ramp1ptr = ramp1, ramp2ptr = ramp2)
        {
            int lastResult = 0;
            var timer = new Stopwatch();

            sb.AppendLine("----==== NO NEON ====----");
            noBurstPerfMarker.Begin();
            timer.Restart();
            lastResult = CalculateDotProd.dotProductScalar(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            noBurstPerfMarker.End();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            // Warm-up. First time running bursted version is slower than the subsequent times
            burstPerfMarker.Begin();
            timer.Restart();
            CalculateDotProd.dotProductBurst(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            burstPerfMarker.End();
            long first = timer.ElapsedMilliseconds;

            sb.AppendLine("----==== NO NEON, Bursted ====----");
            burstPerfMarker.Begin();
            timer.Restart();
            lastResult = CalculateDotProd.dotProductBurst(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            burstPerfMarker.End();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {first}, then {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, no unrolling ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, 2x unrolling ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon2(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, 3x unrolling ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon3(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, 4x unrolling ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon4(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, 6x unrolling ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon6(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, SMLAL+SMLAL2 2-wide ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon_with_SMLAL2_2wide(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== SMLAL+SMLAL2 4-wide ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon_with_SMLAL2_4wide(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();
        }

        return sb.ToString();
    }

    static short[] generateRamp(short startValue, short len)
    {
        var ramp = new short[len];

        for (short i = 0; i < len; i++)
        {
            ramp[i] = (short)(startValue + i);
        }

        return ramp;
    }


    static float[] RandomFloatArray(int length)
    {
        if (length <= 0)
        {
            return new float[0];
        }
        float[] tArr = new float[length];
        for (int i = 0; i < length; i++)
        {
            tArr[i] = Random.Range(minInclude, maxInclude);
        }
        return tArr;
    }

    static float FloatArraySum(float[] array)
    {
        float sum = 0;
        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i];
        }
        return sum;
    }

    string DotTest()
    {

        int length = 1000;
        float[] target = RandomFloatArray(length);
        StringBuilder sb = new StringBuilder();
        _sw.Restart();
        float ranSum1;
        float[] t1 = Test(target, FunctionType.WithoutBurst, targetCount, length, out ranSum1);


        sb.AppendLine("TestData Result WithoutBurst RanSum:");
        sb.AppendLine(ranSum1.ToString());
        sb.AppendLine("TestData Result FloatArraySum:");
        float targetRandomDotSum = FloatArraySum(t1);
        sb.AppendLine(targetRandomDotSum.ToString());

        _sw.Stop();
        sb.AppendLine("Elapsed Milliseconds:");
        sb.AppendLine(_sw.ElapsedMilliseconds.ToString() + " ms");

        _sw.Restart();
        float ranSum2;
        float[] t2 = Test(target, FunctionType.Burst, targetCount, length, out ranSum2);

        StringBuilder sb1 = new StringBuilder();
        sb.AppendLine("TestData Result Burst RanSum:");
        sb.AppendLine(ranSum2.ToString());
        sb.AppendLine("TestData Result FloatArraySum:");
        float targetRandomDotSum1 = FloatArraySum(t2);
        sb.AppendLine(targetRandomDotSum.ToString());

        _sw.Stop();
        sb.AppendLine("Elapsed Milliseconds:");
        sb.AppendLine(_sw.ElapsedMilliseconds.ToString() + " ms");


        _sw.Restart();
        float ranSum3;
        //float[] test = { 1, 2, 3, 4, 5, 6, 7, 8 };
        //target = test;
        //length = test.Length;
        float[] t3 = Test(target, FunctionType.Neon, targetCount, length, out ranSum3);

        StringBuilder sb2 = new StringBuilder();
        sb.AppendLine("TestData Result Neon RanSum:");
        sb.AppendLine(ranSum3.ToString());
        //sb.AppendLine("TestData All Sum Element:");
        //for (int i = 0; i < t3.Length; i++)
        //{
        //    sb.AppendLine(t3[i].ToString());
        //}
        float targetRandomDotSum2 = FloatArraySum(t3);
        sb.AppendLine("TestData Result FloatArraySum:");
        sb.AppendLine(targetRandomDotSum2.ToString());

        _sw.Stop();
        sb.AppendLine("Elapsed Milliseconds:");
        sb.AppendLine(_sw.ElapsedMilliseconds.ToString() + " ms");

        return sb.ToString();
    }
}
