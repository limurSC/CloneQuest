public interface ILevelReadyHandler { void OnLevelReady(); }
public interface ILevelStartHandler { void OnLevelStart(); }
public interface ILevelSoftResetStartHandler { void OnSoftResetStart(float duration); }
public interface ILevelSoftResetEndHandler { void OnSoftResetEnd(); }
public interface IBeforeLevelReloadHandler { void OnBeforeLevelReload(); }
