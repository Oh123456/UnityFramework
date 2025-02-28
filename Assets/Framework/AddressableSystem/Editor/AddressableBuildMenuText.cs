using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AddressableEditor
{
    public static partial class AddressableBuildMenu
    {
        const string BUILD_SETTING_PATH = "Assets/Editor";
        const string BUILD_SETTING_NAME = "AddressableBuildSetting.asset";

        const string BUILD_WARNING = "The IsBuildCilerFolder option is enabled in Editor/AddressableBuildSetting.\r\nDuring the build process, everything except the folder will be deleted before building. \r\n\n Editor/AddressableBuildSetting�� IsBuildCilerFolder �ɼ��� Ȱ��ȭ�� ���ֽ��ϴ�. \r\n����� ������ �����ϰ� ���� �������� �����մϴ�.";
        const string BUILD_BACKUP_PATH_WARNING = "The backUpPath value in Editor/AddressableBuildSetting is empty.\r\nIn this case, the process will automatically proceed in the Documents folder. \r\n\n Editor/AddressableBuildSetting�� backUpPath ���� ��� �ֽ��ϴ�.\r\n�� ��� ���� ������ �ڵ����� ����˴ϴ�. ";
        const string BUILD_BACKUP_RULE_WARNING = "The backUpFolderNameRule value in Editor/AddressableBuildSetting is empty.\r\nIn this case, the current time will be automatically applied. \r\n\n Editor/AddressableBuildSetting�� backUpFodierNameRule ���� ��� �ֽ��ϴ�. \r\n�� ��� ���� �ð����� �ڵ����� ����˴ϴ�.";

    }

}