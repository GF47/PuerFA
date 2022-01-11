"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.genCode = void 0;
const CodeWriter_1 = require("./CodeWriter");
function genCode(handler) {
    let settings = handler.project.GetSettings("Publish").codeGeneration;
    let codePkgName = handler.ToFilename(handler.pkg.name); //convert chinese to pinyin, remove special chars etc.
    let exportCodePath = handler.exportCodePath + '/' + codePkgName;
    let namespaceName = codePkgName;
    let ns = "FairyGUI";
    if (settings.packageName)
        namespaceName = settings.packageName + '.' + namespaceName;
    //CollectClasses(stripeMemeber, stripeClass, fguiNamespace)
    let classes = handler.CollectClasses(settings.ignoreNoname, settings.ignoreNoname, ns);
    handler.SetupCodeFolder(exportCodePath, "ts"); //check if target folder exists, and delete old files
    let getMemberByName = settings.getMemberByName;
    let classCnt = classes.Count;
    let writer = new CodeWriter_1.default({ blockFromNewLine: false, usingTabs: true });
    for (let i = 0; i < classCnt; i++) {
        let classInfo = classes.get_Item(i);
        let members = classInfo.members;
        let references = classInfo.references;
        writer.reset();
        let refCount = references.Count;
        if (refCount > 0) {
            for (let j = 0; j < refCount; j++) {
                let ref = references.get_Item(j);
                writer.writeln('import %s from "./%s";', ref, ref);
            }
            writer.writeln();
        }
        writer.writeln("import { FairyGUI } from \"csharp\";");
        writer.writeln();
        writer.writeln('export default class %s extends %s', classInfo.className, classInfo.superClassName);
        writer.startBlock();
        writer.writeln();
        let memberCnt = members.Count;
        for (let j = 0; j < memberCnt; j++) {
            let memberInfo = members.get_Item(j);
            writer.writeln('public %s: %s;', memberInfo.varName, memberInfo.type);
        }
        writer.writeln('public static URL: string = "ui://%s%s";', handler.pkg.id, classInfo.resId);
        writer.writeln();
        writer.writeln('public static createInstance(): %s', classInfo.className);
        writer.startBlock();
        writer.writeln('return <%s>(%s.UIPackage.CreateObject("%s", "%s"));', classInfo.className, ns, handler.pkg.name, classInfo.resName);
        writer.endBlock();
        writer.writeln();
        writer.writeln('constructor()');
        writer.startBlock();
        writer.writeln('super();');
        writer.writeln('this.__onConstruct = () =>');
        writer.startBlock();
        for (let j = 0; j < memberCnt; j++) {
            let memberInfo = members.get_Item(j);
            if (memberInfo.group == 0) {
                if (getMemberByName)
                    writer.writeln('this.%s = <%s>(this.GetChild("%s"));', memberInfo.varName, memberInfo.type, memberInfo.name);
                else
                    writer.writeln('this.%s = <%s>(this.GetChildAt(%s));', memberInfo.varName, memberInfo.type, memberInfo.index);
            }
            else if (memberInfo.group == 1) {
                if (getMemberByName)
                    writer.writeln('this.%s = this.GetController("%s");', memberInfo.varName, memberInfo.name);
                else
                    writer.writeln('this.%s = this.GetControllerAt(%s);', memberInfo.varName, memberInfo.index);
            }
            else {
                if (getMemberByName)
                    writer.writeln('this.%s = this.GetTransition("%s");', memberInfo.varName, memberInfo.name);
                else
                    writer.writeln('this.%s = this.GetTransitionAt(%s);', memberInfo.varName, memberInfo.index);
            }
        }
        writer.decIndent();
        writer.writeln('};'); // lambda
        writer.endBlock(); // constructor
        writer.endBlock(); //class
        writer.save(exportCodePath + '/' + classInfo.className + '.ts');
    }
    writer.reset();
    let binderName = codePkgName + 'Binder';
    writer.writeln("import { FairyGUI } from \"csharp\";");
    writer.writeln();
    for (let i = 0; i < classCnt; i++) {
        let classInfo = classes.get_Item(i);
        writer.writeln('import %s from "./%s";', classInfo.className, classInfo.className);
    }
    writer.writeln();
    writer.writeln('export default class %s', binderName);
    writer.startBlock();
    writer.writeln('public static register(): void');
    writer.startBlock();
    for (let i = 0; i < classCnt; i++) {
        let classInfo = classes.get_Item(i);
        writer.writeln('%s.UIObjectFactory.SetPackageItemExtension(%s.URL, () => new %s());', ns, classInfo.className, classInfo.className);
    }
    writer.endBlock(); //bindall
    writer.endBlock(); //class
    writer.save(exportCodePath + '/' + binderName + '.ts');
}
exports.genCode = genCode;
