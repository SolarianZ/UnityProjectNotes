# Unity项目备忘

在Unity Editor中显示备忘录的小工具，可用于在团队间分享信息。

> 有没有因为被同事频繁问同一个问题而烦恼过？试试这个。

![Toolbar Entry](./Documents~/imgs/projectnotes_toolbar_entry.gif)

![Main Window](./Documents~/imgs/projectnotes_main_window.png)

![Note Editing Window](./Documents~/imgs/projectnotes_note_editing_window.png)


## 功能

- 在独立的EditorWindow中显示备忘内容
- 备忘录条目支持富文本、历史版本和类别
- 支持添加、修改、删除备忘录条目
- 支持按标题、内容和作者查找备忘录条目
- 将本地配置文件上传到版本控制系统后，即可在团队中分享备忘录
- 备忘录配置中有新增条目时，在Editor工具栏中显示醒目提示 ![Toolbar Entry](./Documents~/imgs/projectnotes_toolbar_entry.gif)


## 支持的Unity版本

Unity 2021.3或更高版本。

部分功能在Unity 2022之前的版本中不能使用，例如：
- 无法使用鼠标选择备忘内容文本。
- 不支持部分富文本标签（例如超链接<a\>）


## 安装

// TODO


## 如何使用

点击Editor工具栏的入口按钮，或者菜单项“Tools/Bamboo/Project Notes”，打开Project Notes窗口。

点击Project Notes窗口右上角的“+”按钮，添加新的备忘录条目。

使用Project Notes窗口顶部的搜索栏，搜索备忘录条目。

使用Project Notes窗口右下角的按钮和下拉按钮，标记、编辑或删除备忘录条目。


## 已知问题

**更改Unity Editor的布局（Layout）后，Editor工具栏中的入口按钮会消失。**

解决方案（任一即可）：

- 在Project Notes窗口的上下文菜单中，选择“Create Toolbar Entry Button”选项，重新创建入口按钮。
- 修改任意代码，触发Unity的域重载。
- 重启Unity Editor。