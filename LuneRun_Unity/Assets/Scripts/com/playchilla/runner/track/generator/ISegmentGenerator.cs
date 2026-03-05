namespace com.playchilla.runner.track.generator
{
    public interface ISegmentGenerator
    {
        bool CanRun(ISegmentGenerator previousGenerator, double difficulty, int levelId);
        void Generate(ISegmentGenerator previousGenerator, double difficulty, int levelId);
    }
}
