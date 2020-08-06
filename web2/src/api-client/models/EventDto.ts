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

import { exists, mapValues } from '../runtime';
import {
    CreationSourceDto,
    CreationSourceDtoFromJSON,
    CreationSourceDtoFromJSONTyped,
    CreationSourceDtoToJSON,
    EffectDto,
    EffectDtoFromJSON,
    EffectDtoFromJSONTyped,
    EffectDtoToJSON,
    EventKind,
    EventKindFromJSON,
    EventKindFromJSONTyped,
    EventKindToJSON,
} from './';

/**
 * 
 * @export
 * @interface EventDto
 */
export interface EventDto {
    /**
     * 
     * @type {number}
     * @memberof EventDto
     */
    readonly id: number;
    /**
     * 
     * @type {CreationSourceDto}
     * @memberof EventDto
     */
    createdBy: CreationSourceDto;
    /**
     * 
     * @type {number}
     * @memberof EventDto
     */
    readonly actingPlayerId?: number | null;
    /**
     * 
     * @type {EventKind}
     * @memberof EventDto
     */
    kind: EventKind;
    /**
     * 
     * @type {Array<EffectDto>}
     * @memberof EventDto
     */
    readonly effects: Array<EffectDto>;
}

export function EventDtoFromJSON(json: any): EventDto {
    return EventDtoFromJSONTyped(json, false);
}

export function EventDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): EventDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'createdBy': CreationSourceDtoFromJSON(json['createdBy']),
        'actingPlayerId': !exists(json, 'actingPlayerId') ? undefined : json['actingPlayerId'],
        'kind': EventKindFromJSON(json['kind']),
        'effects': ((json['effects'] as Array<any>).map(EffectDtoFromJSON)),
    };
}

export function EventDtoToJSON(value?: EventDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'createdBy': CreationSourceDtoToJSON(value.createdBy),
        'kind': EventKindToJSON(value.kind),
    };
}

