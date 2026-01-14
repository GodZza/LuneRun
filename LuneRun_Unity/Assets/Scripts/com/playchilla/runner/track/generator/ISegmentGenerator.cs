namespace com.playchilla.runner.track.generator
{
    public interface ISegmentGenerator
    {
        bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int segmentCount);
        void Generate(ISegmentGenerator previousGenerator, double difficulty, int segmentCount);
    }
}