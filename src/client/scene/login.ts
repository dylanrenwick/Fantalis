import sceneFactory from '../../shared/scene/sceneFactory';

import textInput from '../gameObject/textInput';
import button from '../gameObject/button';
import vector2 from '../../shared/util/vector2';

export default sceneFactory(0, function(s, game) {
    let user = new textInput("txt_username");
    user.transform.pos = new vector2(250, 200);
    user.transform.size = new vector2(300, 40);
    user.placeholder = "Username";
    s.addGameObject(user);

    let pass = new textInput("txt_password");
    pass.transform.pos = new vector2(250, 250);
    pass.transform.size = new vector2(300, 40);
    pass.placeholder = "Password";
    pass.censored = true;
    s.addGameObject(pass);

    let login = new button("btn_login", "Log In");
    login.backgroundColor = "#ffffff";
    login.borderColor = "#000000";
    login.borderSize = 1;
    login.transform.pos = new vector2(300, 300);
    login.transform.size = new vector2(200, 60);
    login.highlightBackground = "#dddddd";
    login.transform.addEventListener("mouseDown", () => game.netManager.login(user.value, pass.value));
    s.addGameObject(login);
});
