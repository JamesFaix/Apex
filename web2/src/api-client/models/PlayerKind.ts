/* tslint:disable */
/* eslint-disable */
/**
 * Apex API
 * API for Apex
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

/**
 * 
 * @export
 * @enum {string}
 */
export enum PlayerKind {
    NUMBER_1 = 1,
    NUMBER_2 = 2,
    NUMBER_3 = 3
}

export function PlayerKindFromJSON(json: any): PlayerKind {
    return PlayerKindFromJSONTyped(json, false);
}

export function PlayerKindFromJSONTyped(json: any, ignoreDiscriminator: boolean): PlayerKind {
    return json as PlayerKind;
}

export function PlayerKindToJSON(value?: PlayerKind | null): any {
    return value as any;
}
