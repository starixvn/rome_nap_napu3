
// Success=0, Error=1, Warning=2
function AlertValidationMessage(containerid, msg, type, controlValidatedId) {
    switch (type) {
        case 0:
            document.getElementById(containerid).innerHTML = "<div class='alertboxscc'><div>" + msg + "</div></div>";
            break;
        case 1:
            document.getElementById(containerid).innerHTML = "<div class='alertboxerr'><div>" + msg + "</div></div>";
            break;
        case 2:
            document.getElementById(containerid).innerHTML = "<div class='alertboxwrn'><div>" + msg + "</div></div>"; ;
            break;
        default:
            break
    }
    Validators.animate(document.getElementById(controlValidatedId));
    document.getElementById(controlValidatedId).focus();
    window.scroll(0, 0);
    parent.scroll(0, 0);
}

function showCloneHint(ptGrid) {
    
    if (ptGrid.copiedItem == null) {
        return;
    }
    var copiedElements = [0, ptGrid.items[0].items[0].items[0].items[0].element,
                                        ptGrid.items[1].items[0].items[0].items[0].element]; // Handicap of Soccer and Basketball
    var hint = Language.COPY_ALL_HINT.Format('<b>' + ptGrid.activeCopiedTitle + '</b>');
    var otherBetTypesOf = Language.OTHER_BET_TYPE_OF.Format(ptGrid.items[ptGrid.copiedItem.sportId - 1].name);
    hint = hint + '<div class="ul-position"><ul><li>' + otherBetTypesOf + '</li>';
    // Find all copied items to generate tooltip
    var count = ptGrid.items.length;
    var top = 0;
    for (var i = ptGrid.copiedItem.sportId; i < count; i++) {
        if (ptGrid.items[i].copiedItem != null && ptGrid.items[i].copiedItem.sportId == ptGrid.copiedItem.sportId) {
            hint += '<li>' + ptGrid.items[i].name + '</li>';
            top += 18;
        }
    }
    hint = hint + '</ul></div>';
    top = top + 40;
    // Show tooltip
    var yPosition = 70;
    if (context.roleId == 4) {
        yPosition = 122;
    }
    show(copiedElements[ptGrid.copiedItem.sportId], hint, 0, 10, yPosition, -10, top, 30000);
}

function show(elementInput, message, xPos, pointerXPos, yPos, pointerYPos, height, timeout) {
    // <span class="hint"><span>This is the message.<span><span class="hint-pointer">&nbsp;</span></span>
    var hint = document.getElementById('fHint');
    var hintPointer = null;
    if (hint == null) {
        var hint = document.createElement("SPAN");
        hint.className = 'hint';
        var hintMsg = document.createElement("SPAN");
        hint.appendChild(hintMsg);
        hintPointer = document.createElement("SPAN");
        hintPointer.className = 'hint-pointerDown';
        hint.appendChild(hintPointer);
        hint.id = 'fHint';
        hint.style.top = '10px';
        hint.style.left = '10px';
        hint.onclick = function () { // Hide onclick
            hint.style.display = 'none';
        };
        document.body.appendChild(hint);
    };
    hintPointer = hint.lastChild;
    hint.firstChild.innerHTML = message;
    if (this.timeOutHideTooltip) {
        clearTimeout(this.timeOutHideTooltip);
        this.timeOutHideTooltip = null;
    };
    if (typeof (timeout) == 'undefined') {
        timeout = 10000;
    }
    this.timeOutHideTooltip = setTimeout(function () {
        if (hint == null) {
            return;
        }
        hint.style.display = 'none';
    }, timeout); // Auto hide in 10s
    if (elementInput != null) {
        if (typeof xPos == 'undefined') {
            xPos = 0;
        }
        if (typeof pointerXPos == 'undefined') {
            pointerXPos = 10;
        }
        if (typeof yPos == 'undefined') {
            yPos = -45;
        }
        if (typeof (pointerYPos) == 'undefined') {
            pointerYPos = 38;
        }
        if (typeof (height) == 'undefined') {
            height = 18;
        }
        hint.style.height = height + 'px';
        hintPointer.style.left = (pointerXPos + 'px');
        hintPointer.style.top = (pointerYPos + 'px');
        var pos = Validators.utils.findPos(elementInput);
        hint.style.left = (pos[0] + xPos) + 'px';
        hint.style.top = (pos[1] + yPos) + 'px';
        elementInput.onchange = function () {
            hint.style.display = 'none';
        };
        elementInput.focus();
    };
    hint.style.display = 'block';
}

function parseBool2(str) {
    var boolmap = {
        'no': false,
        'NO': false,
        'FALSE': false,
        'False': false,
        'false': false,
        'yes': true,
        'YES': true,
        'TRUE': true,
        'True': true,
        'true': true
    };

    return (str in boolmap && boolmap.hasOwnProperty(str)) ?
      boolmap[str] : !!str;
};