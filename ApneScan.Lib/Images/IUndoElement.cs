namespace ApneScan.Images;

public interface IUndoElement
{
    void ApplyUndo();
    void ApplyRedo();
}