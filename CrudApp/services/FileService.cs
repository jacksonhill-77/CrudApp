namespace CrudApp.services;

public interface IFileService
{
    (bool isSuccess, string? message) WriteLinesToFile(List<string> lines, string filePath);
    (bool isSuccess, string? message) WriteLineToFile(string line, string filePath);
    List<string> ReadLinesFromFile(string filePath);
}

public class FileService : IFileService
{

    public (bool isSuccess, string? message) WriteLinesToFile(List<string> lines, string filePath)
    {
        try
        {
            using var outputFile = new StreamWriter(filePath);
            lines.ForEach(outputFile.WriteLine);
            return (true, null);
        }

        catch (Exception ex)
        {
            return (false, $"Error writing lines: {ex.Message}");
        }
    }

    public (bool isSuccess, string? message) WriteLineToFile(string line, string filePath)
    {
        try
        {
            using var outputFile = new StreamWriter(filePath);
            outputFile.WriteLine(line);
            return (true, null);
        }

        catch (Exception ex)
        {
            return (false, $"Error writing lines: {ex.Message}");
        }
    }

    public List<string> ReadLinesFromFile(string filePath)
    {
        return File.ReadAllLines(filePath).ToList();
    }
}