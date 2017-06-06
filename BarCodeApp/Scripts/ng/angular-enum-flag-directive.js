'use strict';

angular.module('enumFlag', []).directive('ngEnumFlag', function () {
    return {
        restrict: 'A',
        scope: {
            value: '=ngEnumFlag',
            model: '=ngEnumModel'
        },
        link: function (scope, element) {
            var checkbox = element[0];
            element.on('change', function () {
                scope.$apply(function () {
                    if (checkbox.checked) {
                        scope.model += scope.value;
                    } else {
                        scope.model -= scope.value;
                    }
                });
            });
            scope.$watch('model', function () {
                checkbox.checked = (scope.model & scope.value) == scope.value;
            });
        }
    };
});