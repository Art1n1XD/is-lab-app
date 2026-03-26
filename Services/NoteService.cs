using IsLabApp.Models;

namespace IsLabApp.Services;

public class NoteService
{
    private readonly List<Note> _notes = new();
    private int _nextId = 1;

    public List<Note> GetAll() => _notes;

    public Note? GetById(int id) => _notes.FirstOrDefault(n => n.Id == id);

    public Note Create(string title, string text)
    {
        var note = new Note
        {
            Id = _nextId++,
            Title = title,
            Text = text,
            CreatedAt = DateTime.UtcNow
        };
        _notes.Add(note);
        return note;
    }

    public bool Delete(int id)
    {
        var note = GetById(id);
        if (note == null) return false;
        return _notes.Remove(note);
    }
}